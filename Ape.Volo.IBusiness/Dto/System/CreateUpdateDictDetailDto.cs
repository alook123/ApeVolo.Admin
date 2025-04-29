using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Attributes;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.System;

/// <summary>
/// 字典详情Dto
/// </summary>
[AutoMapping(typeof(DictDetail), typeof(CreateUpdateDictDetailDto))]
public class CreateUpdateDictDetailDto : BaseEntityDto<long>
{
    /// <summary>
    /// 字典ID
    /// </summary>
    [Display(Name = "Dict.Detail.DictId")]
    [Required(ErrorMessage = "{0}required")]
    public long DictId { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    [Display(Name = "Dict.Detail.Label")]
    [Required(ErrorMessage = "{0}required")]
    public string Label { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    [Display(Name = "Dict.Detail.Value")]
    [Required(ErrorMessage = "{0}required")]
    public string Value { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Display(Name = "Sys.Sort")]
    [Range(1, 999, ErrorMessage = "{0}range{1}{2}")]
    public int DictSort { get; set; }
}
