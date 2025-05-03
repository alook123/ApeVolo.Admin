﻿using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.Extensions;
using Ape.Volo.IBusiness.System;
using Ape.Volo.SharedModel.Dto.Core.System.Dict;
using Ape.Volo.SharedModel.Queries.Common;
using Ape.Volo.SharedModel.Queries.System;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.System;

/// <summary>
/// 字典详情管理
/// </summary>
[Area("Area.DictionaryDetailManagement")]
[Route("/api/dictDetail", Order = 8)]
public class DictDetailController : BaseApiController
{
    #region 字段

    private readonly IDictDetailService _dictDetailService;

    #endregion

    #region 构造函数

    public DictDetailController(IDictDetailService dictDetailService)
    {
        _dictDetailService = dictDetailService;
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 新增字典详情
    /// </summary>
    /// <param name="createUpdateDictDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("Sys.Create")]
    public async Task<ActionResult> Create(
        [FromBody] CreateUpdateDictDetailDto createUpdateDictDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var result = await _dictDetailService.CreateAsync(createUpdateDictDto);
        return Ok(result);
    }


    /// <summary>
    /// 更新字典详情
    /// </summary>
    /// <param name="createUpdateDictDetailDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("Sys.Edit")]
    public async Task<ActionResult> Update(
        [FromBody] CreateUpdateDictDetailDto createUpdateDictDetailDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var result = await _dictDetailService.UpdateAsync(createUpdateDictDetailDto);
        return Ok(result);
    }

    /// <summary>
    /// 删除字典详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    [Description("Sys.Delete")]
    public async Task<ActionResult> Delete(long id)
    {
        if (id.IsNullOrEmpty())
        {
            return Error("id cannot be empty");
        }

        var result = await _dictDetailService.DeleteAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// 查看字典详情列表
    /// </summary>
    /// <param name="dictDetailQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("Sys.Query")]
    public async Task<ActionResult> Query(DictDetailQueryCriteria dictDetailQueryCriteria, Pagination pagination)
    {
        var list = await _dictDetailService.QueryAsync(dictDetailQueryCriteria, pagination);
        return JsonContent(list, pagination);
    }

    #endregion
}
