using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using Ape.Volo.IBusiness.QueryModel;
using Ape.Volo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.Permission;

/// <summary>
/// 用户管理
/// </summary>
[Area("用户管理")]
[Route("/api/user", Order = 1)]
public class UserController : BaseApiController
{
    #region 字段

    private readonly IUserService _userService;

    #endregion

    #region 构造函数

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 新增用户
    /// </summary>
    /// <param name="createUpdateUserDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Description("创建")]
    [Route("create")]
    public async Task<ActionResult> Create([FromBody] CreateUpdateUserDto createUpdateUserDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var result = await _userService.CreateAsync(createUpdateUserDto);

        return Ok(result);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="createUpdateUserDto"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [HttpPut]
    [Description("编辑")]
    [Route("edit")]
    public async Task<ActionResult> Update([FromBody] CreateUpdateUserDto createUpdateUserDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var result = await _userService.UpdateAsync(createUpdateUserDto);
        return Ok(result);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="idCollection"></param>
    /// <returns></returns>
    [HttpDelete]
    [Description("删除")]
    [Route("delete")]
    public async Task<ActionResult> Delete([FromBody] IdCollection idCollection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var result = await _userService.DeleteAsync(idCollection.IdArray);
        return Ok(result);
    }

    [HttpPut]
    [Route("update/center")]
    [Description("更新个人信息")]
    public async Task<ActionResult> UpdateCenterAsync([FromBody] UpdateUserCenterDto updateUserCenterDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var result = await _userService.UpdateCenterAsync(updateUserCenterDto);
        return Ok(result);
    }

    [HttpPost]
    [Route("update/password")]
    [Description("更新密码")]
    public async Task<ActionResult> UpdatePasswordAsync([FromBody] UpdateUserPassDto updateUserPassDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var result = await _userService.UpdatePasswordAsync(updateUserPassDto);
        return Ok(result);
    }

    [HttpPost]
    [Route("update/email")]
    [Description("更新邮箱")]
    public async Task<ActionResult> UpdateEmail([FromBody] UpdateUserEmailDto updateUserEmailDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var result = await _userService.UpdateEmailAsync(updateUserEmailDto);
        return Ok(result);
    }

    [HttpPost, HttpOptions]
    [Route("update/avatar")]
    [Description("更新头像")]
    public async Task<ActionResult> UpdateAvatar([FromForm] IFormFile avatar) //多文件使用  IFormFileCollection
    {
        if (avatar.IsNull())
        {
            return Error();
        }

        var result = await _userService.UpdateAvatarAsync(avatar);
        return Ok(result);
    }


    /// <summary>
    /// 查看用户列表
    /// </summary>
    /// <param name="userQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("查询")]
    [Route("query")]
    public async Task<ActionResult> Query(UserQueryCriteria userQueryCriteria,
        Pagination pagination)
    {
        var list = await _userService.QueryAsync(userQueryCriteria, pagination);
        return JsonContent(list, pagination);
    }

    /// <summary>
    /// 导出用户列表
    /// </summary>
    /// <param name="userQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("导出")]
    [Route("download")]
    public async Task<ActionResult> Download(UserQueryCriteria userQueryCriteria)
    {
        var userExports = await _userService.DownloadAsync(userQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(userExports, out var mimeType, out var fileName);
        return new FileContentResult(data, mimeType)
        {
            FileDownloadName = fileName
        };
    }

    #endregion
}
