using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Attributes;
using Ape.Volo.Common.Enums;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.Permission;

/// <summary>
/// 角色Dto
/// </summary>
[AutoMapping(typeof(Role), typeof(CreateUpdateRoleDto))]
public class CreateUpdateRoleDto : BaseEntityDto<long>
{
    /// <summary>
    /// 名称
    /// </summary>
    [Display(Name = "Sys.Name")]
    [Required(ErrorMessage = "{0}required")]
    public string Name { get; set; }

    /// <summary>
    /// 等级
    /// </summary>
    [Display(Name = "Role.Level")]
    [Range(1, 99, ErrorMessage = "{0}range{1}{2}")]
    public int Level { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [Display(Name = "Sys.Description")]
    [Required(ErrorMessage = "{0}required")]
    public string Description { get; set; }

    /// <summary>
    /// 数据权限
    /// </summary>
    [Display(Name = "Role.DataScopeType")]
    [Range(0, 5, ErrorMessage = "{0}range{1}{2}")]
    public DataScopeType DataScopeType { get; set; }

    /// <summary>
    /// 标识
    /// </summary>
    [Display(Name = "Role.Permission")]
    [Required(ErrorMessage = "{0}required")]
    public string Permission { get; set; }

    /// <summary>
    /// 角色部门
    /// </summary>
    public List<RoleDeptDto> Depts { get; set; }
}
