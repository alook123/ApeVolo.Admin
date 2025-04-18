using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.ActionExtension.Parameter;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.Interface.System;
using Ape.Volo.IBusiness.QueryModel;
using Ape.Volo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.System;

/// <summary>
/// 文件存储管理
/// </summary>
[Area("文件存储管理")]
[Route("/api/storage", Order = 12)]
public class FileRecordController : BaseApiController
{
    #region 字段

    private readonly IFileRecordService _fileRecordService;

    #endregion

    #region 构造函数

    public FileRecordController(IFileRecordService fileRecordService)
    {
        _fileRecordService = fileRecordService;
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 新增文件
    /// </summary>
    /// <param name="file"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    [HttpPost, HttpOptions]
    [Route("upload")]
    [Description("创建")]
    [CheckParamNotEmpty("description")]
    public async Task<ActionResult> Upload(string description,
        [FromForm] IFormFile file)
    {
        if (file.IsNull())
        {
            return Error("请选择一个文件再尝试!");
        }

        var fileLimitSize = App.GetOptions<SystemOptions>().FileLimitSize * 1024 * 1024;
        if (file.Length > fileLimitSize)
        {
            return Error($"文件过大，请选择文件小于等于{fileLimitSize}MB的重新进行尝试!");
        }

        var result = await _fileRecordService.CreateAsync(description, file);
        return Ok(result);
    }

    /// <summary>
    /// 更新文件描述
    /// </summary>
    /// <param name="createUpdateAppSecretDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("编辑")]
    public async Task<ActionResult> Update(
        [FromBody] CreateUpdateFileRecordDto createUpdateAppSecretDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var result = await _fileRecordService.UpdateAsync(createUpdateAppSecretDto);
        return Ok(result);
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="idCollection"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    [Description("删除")]
    public async Task<ActionResult> Delete([FromBody] IdCollection idCollection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var result = await _fileRecordService.DeleteAsync(idCollection.IdArray);
        return Ok(result);
    }


    /// <summary>
    /// 获取文件列表
    /// </summary>
    /// <param name="fileRecordQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("查询")]
    public async Task<ActionResult> Query(FileRecordQueryCriteria fileRecordQueryCriteria,
        Pagination pagination)
    {
        var fileRecordList = await _fileRecordService.QueryAsync(fileRecordQueryCriteria, pagination);

        return JsonContent(fileRecordList, pagination);
    }


    /// <summary>
    /// 导出文件记录
    /// </summary>
    /// <param name="fileRecordQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("导出")]
    [Route("download")]
    public async Task<ActionResult> Download(FileRecordQueryCriteria fileRecordQueryCriteria)
    {
        var fileRecordExports = await _fileRecordService.DownloadAsync(fileRecordQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(fileRecordExports, out var mimeType, out var fileName);
        return new FileContentResult(data, mimeType)
        {
            FileDownloadName = fileName
        };
    }

    #endregion
}
