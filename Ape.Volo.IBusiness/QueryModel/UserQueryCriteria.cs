using Ape.Volo.Common.Attributes;
using Ape.Volo.Common.Model;
using Newtonsoft.Json;
using SqlSugar;

namespace Ape.Volo.IBusiness.QueryModel;

/// <summary>
/// 用户查询参数
/// </summary>
public class UserQueryCriteria : DateRange, IConditionalModel
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Equal)]
    public long Id { get; set; }

    /// <summary>
    /// 关键字
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like, FieldNameItems = ["Username", "NickName", "Email"])]
    public string KeyWords { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Equal)]
    public bool? Enabled { get; set; }


    /// <summary>
    /// 部门ID
    /// </summary>
    public long DeptId { get; set; }


    /// <summary>
    /// 部门ID集合 用于查询
    /// </summary>
    [JsonIgnore]
    [QueryCondition(ConditionType = ConditionalType.In, FieldName = "DeptId")]
    public string DeptIdItems { get; set; }
}
