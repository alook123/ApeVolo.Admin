using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.Extensions;
using Ape.Volo.IBusiness.Message.Email;
using Ape.Volo.SharedModel.Dto.Core.Message.Email;
using Ape.Volo.SharedModel.Queries.Common;
using Ape.Volo.SharedModel.Queries.Message;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.Message.Email;

/// <summary>
/// 邮件模板管理
/// </summary>
[Area("Area.EmailMessageTemplateManagement")]
[Route("/api/email/template", Order = 18)]
public class EmailMessageTemplateController : BaseApiController
{
    #region 字段

    private readonly IEmailMessageTemplateService _emailMessageTemplateService;

    #endregion

    #region 构造函数

    public EmailMessageTemplateController(IEmailMessageTemplateService emailMessageTemplateService)
    {
        _emailMessageTemplateService = emailMessageTemplateService;
    }

    #endregion

    #region API

    /// <summary>
    /// 新增邮箱账户
    /// </summary>
    /// <param name="createUpdateEmailMessageTemplateDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("Sys.Create")]
    public async Task<ActionResult> Create(
        [FromBody] CreateUpdateEmailMessageTemplateDto createUpdateEmailMessageTemplateDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var result = await _emailMessageTemplateService.CreateAsync(createUpdateEmailMessageTemplateDto);
        return Ok(result);
    }

    /// <summary>
    /// 更新邮箱账户
    /// </summary>
    /// <param name="createUpdateEmailMessageTemplateDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("Sys.Edit")]
    public async Task<ActionResult> Update(
        [FromBody] CreateUpdateEmailMessageTemplateDto createUpdateEmailMessageTemplateDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var result = await _emailMessageTemplateService.UpdateAsync(createUpdateEmailMessageTemplateDto);
        return Ok(result);
    }

    /// <summary>
    /// 删除邮箱账户
    /// </summary>
    /// <param name="idCollection"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    [Description("Sys.Delete")]
    public async Task<ActionResult> Delete([FromBody] IdCollection idCollection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var result = await _emailMessageTemplateService.DeleteAsync(idCollection.IdArray);
        return Ok(result);
    }

    /// <summary>
    /// 邮箱账户列表
    /// </summary>
    /// <param name="messageTemplateQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("Sys.Query")]
    public async Task<ActionResult> Query(EmailMessageTemplateQueryCriteria messageTemplateQueryCriteria,
        Pagination pagination)
    {
        var emailMessageTemplateList =
            await _emailMessageTemplateService.QueryAsync(messageTemplateQueryCriteria, pagination);

        return JsonContent(emailMessageTemplateList, pagination);
    }

    #endregion
}
