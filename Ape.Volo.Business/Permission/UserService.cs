using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
using Ape.Volo.Common;
using Ape.Volo.Common.Attributes;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.Common.Exception;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.IdGenerator;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.ExportModel.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using Ape.Volo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Http;
using SqlSugar;

namespace Ape.Volo.Business.Permission;

/// <summary>
/// 用户服务
/// </summary>
public class UserService : BaseServices<User>, IUserService
{
    #region 字段

    private readonly IDepartmentService _departmentService;
    private readonly IRoleService _roleService;

    #endregion

    #region 构造函数

    public UserService(IDepartmentService departmentService, IRoleService roleService)
    {
        _departmentService = departmentService;
        _roleService = roleService;
    }

    #endregion

    #region 基础方法

    [UseTran]
    public async Task<OperateResult> CreateAsync(CreateUpdateUserDto createUpdateUserDto)
    {
        if (await TableWhere(x => x.Username == createUpdateUserDto.Username).AnyAsync())
        {
            return OperateResult.Error(DataErrorHelper.IsExist(createUpdateUserDto,
                nameof(createUpdateUserDto.Username)));
        }

        if (await TableWhere(x => x.Email == createUpdateUserDto.Email).AnyAsync())
        {
            return OperateResult.Error(DataErrorHelper.IsExist(createUpdateUserDto,
                nameof(createUpdateUserDto.Email)));
        }

        if (await TableWhere(x => x.Phone == createUpdateUserDto.Phone).AnyAsync())
        {
            return OperateResult.Error(DataErrorHelper.IsExist(createUpdateUserDto,
                nameof(createUpdateUserDto.Phone)));
        }

        var user = App.Mapper.MapTo<User>(createUpdateUserDto);

        //设置用户密码
        user.Password = BCryptHelper.Hash(App.GetOptions<SystemOptions>().UserDefaultPassword);
        user.DeptId = user.Dept.Id;
        //用户
        await AddAsync(user);


        await SugarClient.Deleteable<UserRole>().Where(x => x.UserId == user.Id).ExecuteCommandAsync();
        var userRoles = new List<UserRole>();
        userRoles.AddRange(user.Roles.Select(x => new UserRole { UserId = user.Id, RoleId = x.Id }));
        await SugarClient.Insertable(userRoles).ExecuteCommandAsync();


        await SugarClient.Deleteable<UserJob>().Where(x => x.UserId == user.Id).ExecuteCommandAsync();
        var userJobs = new List<UserJob>();
        userJobs.AddRange(user.Jobs.Select(x => new UserJob { UserId = user.Id, JobId = x.Id }));
        await SugarClient.Insertable(userJobs).ExecuteCommandAsync();

        return OperateResult.Success();
    }

    [UseTran]
    public async Task<OperateResult> UpdateAsync(CreateUpdateUserDto createUpdateUserDto)
    {
        //取出待更新数据
        var oldUser = await TableWhere(x => x.Id == createUpdateUserDto.Id).Includes(x => x.Roles).FirstAsync();
        if (oldUser.IsNull())
        {
            return OperateResult.Error(DataErrorHelper.NotExist(createUpdateUserDto, LanguageKeyConstants.User,
                nameof(createUpdateUserDto.Id)));
        }

        if (oldUser.Username != createUpdateUserDto.Username &&
            await TableWhere(x => x.Username == createUpdateUserDto.Username).AnyAsync())
        {
            return OperateResult.Error(DataErrorHelper.IsExist(createUpdateUserDto,
                nameof(createUpdateUserDto.Username)));
        }

        if (oldUser.Email != createUpdateUserDto.Email &&
            await TableWhere(x => x.Email == createUpdateUserDto.Email).AnyAsync())
        {
            return OperateResult.Error(DataErrorHelper.IsExist(createUpdateUserDto,
                nameof(createUpdateUserDto.Email)));
        }

        if (oldUser.Phone != createUpdateUserDto.Phone &&
            await TableWhere(x => x.Phone == createUpdateUserDto.Phone).AnyAsync())
        {
            return OperateResult.Error(DataErrorHelper.IsExist(createUpdateUserDto,
                nameof(createUpdateUserDto.Phone)));
        }

        //验证角色等级
        var levels = oldUser.Roles.Select(x => x.Level);
        await _roleService.VerificationUserRoleLevelAsync(levels.Min());
        var user = App.Mapper.MapTo<User>(createUpdateUserDto);
        user.DeptId = user.Dept.Id;
        //更新用户
        await UpdateAsync(user, null, x => new { x.Password, x.AvatarPath, x.IsAdmin, x.PasswordReSetTime });


        await SugarClient.Deleteable<UserRole>().Where(x => x.UserId == user.Id).ExecuteCommandAsync();
        var userRoles = new List<UserRole>();
        userRoles.AddRange(user.Roles.Select(x => new UserRole { UserId = user.Id, RoleId = x.Id }));
        await SugarClient.Insertable(userRoles).ExecuteCommandAsync();


        await SugarClient.Deleteable<UserJob>().Where(x => x.UserId == user.Id).ExecuteCommandAsync();
        var userJobs = new List<UserJob>();
        userJobs.AddRange(user.Jobs.Select(x => new UserJob { UserId = user.Id, JobId = x.Id }));
        await SugarClient.Insertable(userJobs).ExecuteCommandAsync();

        //清理缓存
        await ClearUserCache(user.Id);
        return OperateResult.Success();
    }

    public async Task<OperateResult> DeleteAsync(HashSet<long> ids)
    {
        if (ids.Contains(App.HttpUser.Id))
        {
            return OperateResult.Error(App.L.R("Error.ForbidToDeleteYourself"));
        }

        //验证角色等级
        await _roleService.VerificationUserRoleLevelAsync(await _roleService.QueryUserRoleLevelAsync(ids));


        var users = await TableWhere(x => ids.Contains(x.Id)).ToListAsync();
        foreach (var user in users)
        {
            await ClearUserCache(user.Id);
        }

        var result = await LogicDelete<User>(x => ids.Contains(x.Id));
        return OperateResult.Result(result);
    }

    /// <summary>
    /// 用户列表
    /// </summary>
    /// <param name="userQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    public async Task<List<UserDto>> QueryAsync(UserQueryCriteria userQueryCriteria, Pagination pagination)
    {
        var conditionalModels = await GetConditionalModel(userQueryCriteria);
        var queryOptions = new QueryOptions<User>
        {
            Pagination = pagination,
            ConditionalModels = conditionalModels,
            IsIncludes = true
        };
        var users = await TablePageAsync(queryOptions);

        return App.Mapper.MapTo<List<UserDto>>(users);
    }


    public async Task<List<ExportBase>> DownloadAsync(UserQueryCriteria userQueryCriteria)
    {
        var conditionalModels = await GetConditionalModel(userQueryCriteria);
        var users = await Table.Includes(x => x.Dept).Includes(x => x.Roles)
            .Includes(x => x.Jobs).Where(conditionalModels).ToListAsync();
        List<ExportBase> userExports = new List<ExportBase>();
        userExports.AddRange(users.Select(x => new UserExport
        {
            Id = x.Id,
            Username = x.Username,
            Role = string.Join(",", x.Roles.Select(r => r.Name).ToArray()),
            NickName = x.NickName,
            Phone = x.Phone,
            Email = x.Email,
            Enabled = x.Enabled,
            Dept = x.Dept.Name,
            Job = string.Join(",", x.Jobs.Select(j => j.Name).ToArray()),
            Gender = x.Gender,
            CreateTime = x.CreateTime
        }));
        return userExports;
    }

    #endregion

    #region 扩展方法

    //[UseCache(Expiration = 60, KeyPrefix = GlobalConstants.CachePrefix.UserInfoById)]
    public async Task<UserDto> QueryByIdAsync(long userId)
    {
        var user = await TableWhere(x => x.Id == userId, null, null, null, true).Includes(x => x.Dept)
            .Includes(x => x.Roles)
            .Includes(x => x.Jobs).FirstAsync();

        return App.Mapper.MapTo<UserDto>(user);
    }

    /// <summary>
    /// 查询用户
    /// </summary>
    /// <param name="userName">邮箱 or 用户名</param>
    /// <returns></returns>
    public async Task<UserDto> QueryByNameAsync(string userName)
    {
        User user;
        if (userName.IsEmail())
        {
            user = await TableWhere(s => s.Email == userName, null, null, null, true).FirstAsync();
        }
        else
        {
            user = await TableWhere(s => s.Username == userName, null, null, null, true).FirstAsync();
        }

        return App.Mapper.MapTo<UserDto>(user);
    }

    /// <summary>
    /// 根据部门ID查找用户
    /// </summary>
    /// <param name="deptIds"></param>
    /// <returns></returns>
    public async Task<List<UserDto>> QueryByDeptIdsAsync(List<long> deptIds)
    {
        return App.Mapper.MapTo<List<UserDto>>(
            await TableWhere(u => deptIds.Contains(u.DeptId)).ToListAsync());
    }

    /// <summary>
    /// 更新用户公共信息
    /// </summary>
    /// <param name="updateUserCenterDto"></param>
    /// <returns></returns>
    /// <exception cref="BadRequestException"></exception>
    public async Task<OperateResult> UpdateCenterAsync(UpdateUserCenterDto updateUserCenterDto)
    {
        var user = await TableWhere(x => x.Id == App.HttpUser.Id).FirstAsync();
        if (user.IsNull())
        {
            return OperateResult.Error(DataErrorHelper.NotExist());
        }

        var checkUser = await TableWhere(x =>
            x.Phone == updateUserCenterDto.Phone && x.Id != App.HttpUser.Id).FirstAsync();
        if (checkUser.IsNotNull())
        {
            return OperateResult.Error(DataErrorHelper.IsExist(updateUserCenterDto,
                nameof(updateUserCenterDto.Phone)));
        }

        user.NickName = updateUserCenterDto.NickName;
        user.Gender = updateUserCenterDto.Gender;
        user.Phone = updateUserCenterDto.Phone;
        var result = await UpdateAsync(user);
        return OperateResult.Result(result);
    }

    public async Task<OperateResult> UpdatePasswordAsync(UpdateUserPassDto userPassDto)
    {
        var rsaHelper = new RsaHelper(App.GetOptions<RsaOptions>());
        string oldPassword = rsaHelper.Decrypt(userPassDto.OldPassword);
        string newPassword = rsaHelper.Decrypt(userPassDto.NewPassword);
        string confirmPassword = rsaHelper.Decrypt(userPassDto.ConfirmPassword);

        if (oldPassword == newPassword)
            return OperateResult.Error(App.L.R("Error.PasswordSameAsOld"));

        if (!newPassword.Equals(confirmPassword))
        {
            return OperateResult.Error(App.L.R("Error.InputsDoNotMatch"));
        }

        var curUser = await TableWhere(x => x.Id == App.HttpUser.Id).FirstAsync();
        if (curUser.IsNull())
        {
            return OperateResult.Error(DataErrorHelper.NotExist());
        }

        if (!BCryptHelper.Verify(oldPassword, curUser.Password))
        {
            return OperateResult.Error(App.L.R("Error.IncorrectOldPassword"));
        }

        //设置用户密码
        curUser.Password = BCryptHelper.Hash(newPassword);
        curUser.PasswordReSetTime = DateTime.Now;
        var isTrue = await UpdateAsync(curUser);
        if (isTrue)
        {
            //清理缓存
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserInfoById +
                                        curUser.Id.ToString().ToMd5String16());

            //退出当前用户
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.OnlineKey +
                                        App.HttpUser.JwtToken.ToMd5String16());
        }

        return OperateResult.Success();
    }

    /// <summary>
    /// 修改邮箱
    /// </summary>
    /// <param name="updateUserEmailDto"></param>
    /// <returns></returns>
    public async Task<OperateResult> UpdateEmailAsync(UpdateUserEmailDto updateUserEmailDto)
    {
        var curUser = await TableWhere(x => x.Id == App.HttpUser.Id).FirstAsync();
        if (curUser.IsNull())
        {
            return OperateResult.Error(DataErrorHelper.NotExist());
        }

        var rsaHelper = new RsaHelper(App.GetOptions<RsaOptions>());
        string password = rsaHelper.Decrypt(updateUserEmailDto.Password);
        if (!BCryptHelper.Verify(password, curUser.Password))
        {
            return OperateResult.Error(App.L.R("Error.InvalidPassword"));
        }

        var code = await App.Cache.GetAsync<string>(
            GlobalConstants.CachePrefix.EmailCaptcha + updateUserEmailDto.Email.ToMd5String16());
        if (code.IsNullOrEmpty() || !code.Equals(updateUserEmailDto.Code))
        {
            return OperateResult.Error(App.L.R("Error.InvalidVerificationCode"));
        }

        curUser.Email = updateUserEmailDto.Email;
        var result = await UpdateAsync(curUser);
        return OperateResult.Result(result);
    }

    public async Task<OperateResult> UpdateAvatarAsync(IFormFile file)
    {
        var curUser = await TableWhere(x => x.Id == App.HttpUser.Id).FirstAsync();
        if (curUser.IsNull())
        {
            return OperateResult.Error(DataErrorHelper.NotExist());
        }


        var prefix = App.WebHostEnvironment.WebRootPath;
        string avatarName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + IdHelper.NextId() +
                            file.FileName.Substring(Math.Max(file.FileName.LastIndexOf('.'), 0));
        string avatarPath = Path.Combine(prefix, "uploads", "file", "avatar");

        if (!Directory.Exists(avatarPath))
        {
            Directory.CreateDirectory(avatarPath);
        }

        avatarPath = Path.Combine(avatarPath, avatarName);
        await using (var fs = new FileStream(avatarPath, FileMode.CreateNew))
        {
            await file.CopyToAsync(fs);
            fs.Flush();
        }

        string relativePath = Path.GetRelativePath(prefix, avatarPath);
        relativePath = "/" + relativePath.Replace("\\", "/");
        curUser.AvatarPath = relativePath;
        await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserInfoById +
                                    curUser.Id.ToString().ToMd5String16());
        var result = await UpdateAsync(curUser);
        return OperateResult.Result(result);
    }

    #endregion

    #region 用户缓存

    private async Task ClearUserCache(long userId)
    {
        //清理缓存
        await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserInfoById +
                                    userId.ToString().ToMd5String16());
        await App.Cache.RemoveAsync(
            GlobalConstants.CachePrefix.UserPermissionUrls + userId.ToString().ToMd5String16());
        await App.Cache.RemoveAsync(
            GlobalConstants.CachePrefix.UserPermissionRoles + userId.ToString().ToMd5String16());
        await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserMenuById +
                                    userId.ToString().ToMd5String16());
        await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserDataScopeById +
                                    userId.ToString().ToMd5String16());
    }

    #endregion

    #region 条件模型

    private async Task<List<IConditionalModel>> GetConditionalModel(UserQueryCriteria userQueryCriteria)
    {
        if (userQueryCriteria.DeptId > 0)
        {
            var allIds = await _departmentService.GetChildIds([userQueryCriteria.DeptId], null);
            if (allIds.Any())
            {
                userQueryCriteria.DeptIdItems = string.Join(",", allIds);
            }
        }

        return userQueryCriteria.ApplyQueryConditionalModel();
    }

    #endregion
}
