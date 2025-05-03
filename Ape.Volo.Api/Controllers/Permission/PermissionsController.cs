using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Permission;
using Ape.Volo.SharedModel.Dto.Core.Permission.Role;
using Ape.Volo.ViewModel.Core.Permission.Menu;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.Permission;

/// <summary>
/// 角色授权管理
/// </summary>
[Area("Area.RoleAuthorizationManagement")]
[Route("/api/permissions", Order = 3)]
public class PermissionsController : BaseApiController
{
    #region 字段

    private readonly IRoleService _roleService;
    private readonly IMenuService _menuService;
    private readonly IApisService _apisService;

    #endregion

    #region 构造函数

    public PermissionsController(IRoleService roleService, IMenuService menuService, IApisService apisService)
    {
        _roleService = roleService;
        _menuService = menuService;
        _apisService = apisService;
    }

    #endregion

    #region 对内接口

    /// <summary>
    /// 查询所有菜单
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("menus/query")]
    [Description("Action.GetAllMenu")]
    public async Task<ActionResult> QueryAllMenus()
    {
        var menus = await _menuService.QueryAllAsync();

        var menuTree = TreeHelper<MenuVo>.ListToTrees(menus, "Id", "ParentId", 0);
        return JsonContent(menuTree);
    }


    /// <summary>
    /// 查询所有Api
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("apis/query")]
    [Description("Action.GetAllApi")]
    public async Task<ActionResult> QueryAllApis()
    {
        List<ApisTree> apisTree = new List<ApisTree>();
        var apis = await _apisService.QueryAllAsync();
        var apisGroup = apis.GroupBy(x => x.Group).ToList();

        var index = 0;
        foreach (var g in apisGroup)
        {
            var apisTreesTmp = new List<ApisTree>();
            foreach (var api in g.ToList())
            {
                apisTreesTmp.Add(new ApisTree
                {
                    Id = api.Id,
                    Label = api.Description,
                    Leaf = true,
                    HasChildren = false,
                    Children = null
                });
            }

            index++;
            apisTree.Add(new ApisTree
            {
                Id = index,
                Label = g.Key,
                Leaf = false,
                HasChildren = true,
                Children = apisTreesTmp
            });
        }

        return JsonContent(apisTree);
    }


    /// <summary>
    /// 更新角色菜单关联
    /// </summary>
    /// <param name="updateRoleMenuDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("menus/edit")]
    [Description("Action.UpdateRoleMenu")]
    public async Task<ActionResult> UpdateRolesMenus([FromBody] UpdateRoleMenuDto updateRoleMenuDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var result = await _roleService.UpdateRolesMenusAsync(updateRoleMenuDto);
        return Ok(result);
    }

    /// <summary>
    /// 更新角色Api关联
    /// </summary>
    /// <param name="updateRoleApiDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("apis/edit")]
    [Description("Action.UpdateRoleApi")]
    public async Task<ActionResult> UpdateRolesApis([FromBody] UpdateRoleApiDto updateRoleApiDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var result = await _roleService.UpdateRolesApisAsync(updateRoleApiDto);
        return Ok(result);
    }

    #endregion
}
