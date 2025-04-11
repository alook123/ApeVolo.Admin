using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common;
using Ape.Volo.Common.Enums;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.Interface.System;
using Ape.Volo.IBusiness.QueryModel;
using Ape.Volo.IBusiness.RequestModel;
using Ape.Volo.QuartzNetService.service;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace Ape.Volo.Api.Controllers.System;

/// <summary>
/// 作业调度管理
/// </summary>
[Area("作业调度管理")]
[Route("/api/tasks", Order = 9)]
public class QuartzNetController : BaseApiController
{
    #region 字段

    private readonly IQuartzNetService _quartzNetService;
    private readonly IQuartzNetLogService _quartzNetLogService;
    private readonly ISchedulerCenterService _schedulerCenterService;

    #endregion

    #region 构造函数

    public QuartzNetController(IQuartzNetService quartzNetService, IQuartzNetLogService quartzNetLogService,
        ISchedulerCenterService schedulerCenterService)
    {
        _quartzNetService = quartzNetService;
        _quartzNetLogService = quartzNetLogService;
        _schedulerCenterService = schedulerCenterService;
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 新增作业
    /// </summary>
    /// <param name="createUpdateQuartzNetDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("创建")]
    public async Task<ActionResult> Create(
        [FromBody] CreateUpdateQuartzNetDto createUpdateQuartzNetDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        if (createUpdateQuartzNetDto.TriggerType == (int)TriggerType.Cron)
        {
            if (createUpdateQuartzNetDto.Cron.IsNullOrEmpty())
            {
                return Error("cron模式下请设置作业执行cron表达式");
            }

            if (!CronExpression.IsValidExpression(createUpdateQuartzNetDto.Cron))
            {
                return Error("cron模式下请设置正确得cron表达式");
            }
        }
        else if (createUpdateQuartzNetDto.TriggerType == (int)TriggerType.Simple)
        {
            if (createUpdateQuartzNetDto.IntervalSecond <= 5)
            {
                return Error("simple模式下请设置作业间隔执行秒数");
            }
        }

        var quartzNet = await _quartzNetService.CreateAsync(createUpdateQuartzNetDto);
        if (quartzNet.IsNotNull())
        {
            if (quartzNet.IsEnable)
            {
                //开启作业任务
                await _schedulerCenterService.AddScheduleJobAsync(quartzNet);
            }
        }

        return Ok(OperateResult.Result(quartzNet.IsNotNull()));
    }

    /// <summary>
    /// 更新作业
    /// </summary>
    /// <param name="createUpdateQuartzNetDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("编辑")]
    public async Task<ActionResult> Update(
        [FromBody] CreateUpdateQuartzNetDto createUpdateQuartzNetDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        if (createUpdateQuartzNetDto.TriggerType == (int)TriggerType.Cron)
        {
            if (createUpdateQuartzNetDto.Cron.IsNullOrEmpty())
            {
                return Error("cron模式下请设置作业执行cron表达式");
            }

            if (!CronExpression.IsValidExpression(createUpdateQuartzNetDto.Cron))
            {
                return Error("cron模式下请设置正确得cron表达式");
            }
        }
        else if (createUpdateQuartzNetDto.TriggerType == (int)TriggerType.Simple)
        {
            if (createUpdateQuartzNetDto.IntervalSecond <= 5)
            {
                return Error("simple模式下请设置作业间隔执行秒数");
            }
        }

        var result = await _quartzNetService.UpdateAsync(createUpdateQuartzNetDto);
        if (result.IsSuccess)
        {
            var quartzNet = App.Mapper.MapTo<QuartzNet>(createUpdateQuartzNetDto);
            await _schedulerCenterService.DeleteScheduleJobAsync(quartzNet);

            if (quartzNet.IsEnable)
            {
                await _schedulerCenterService.AddScheduleJobAsync(quartzNet);
            }
        }

        return Ok(result);
    }

    /// <summary>
    /// 删除作业
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

        var quartzList = await _quartzNetService.TableWhere(x => idCollection.IdArray.Contains(x.Id)).ToListAsync();
        if (quartzList.Count > 0)
        {
            var result = await _quartzNetService.DeleteAsync(quartzList);
            if (result.IsSuccess)
            {
                foreach (var item in quartzList)
                {
                    await _schedulerCenterService.DeleteScheduleJobAsync(item);
                }
            }

            return Ok(result);
        }

        return Error();
    }

    /// <summary>
    /// 获取作业调度任务列表
    /// </summary>
    /// <param name="quartzNetQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("查询")]
    public async Task<ActionResult> Query(QuartzNetQueryCriteria quartzNetQueryCriteria,
        Pagination pagination)
    {
        var quartzNetList = await _quartzNetService.QueryAsync(quartzNetQueryCriteria, pagination);

        foreach (var quartzNet in quartzNetList)
        {
            quartzNet.TriggerStatus = await _schedulerCenterService.GetTriggerStatus(quartzNet);
        }

        return JsonContent(quartzNetList, pagination);
    }

    /// <summary>
    /// 导出作业调度
    /// </summary>
    /// <param name="quartzNetQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("导出")]
    [Route("download")]
    public async Task<ActionResult> Download(QuartzNetQueryCriteria quartzNetQueryCriteria)
    {
        var quartzNetExports = await _quartzNetService.DownloadAsync(quartzNetQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(quartzNetExports, out var mimeType, out var fileName);
        return new FileContentResult(data, mimeType)
        {
            FileDownloadName = fileName
        };
    }


    /// <summary>
    /// 执行作业
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut]
    [Description("执行")]
    [Route("execute")]
    public async Task<ActionResult> Execute(long id)
    {
        if (id.IsNullOrEmpty())
        {
            return Error("id cannot be empty");
        }

        var quartzNet = await _quartzNetService.TableWhere(x => x.Id == id).FirstAsync();
        if (quartzNet.IsNull())
        {
            return Error("作业调度不存在");
        }

        //开启作业任务
        quartzNet.IsEnable = true;
        if (await _quartzNetService.UpdateAsync(quartzNet))
        {
            //检查任务在内存状态
            var isTrue = await _schedulerCenterService.IsExistScheduleJobAsync(quartzNet);
            if (!isTrue)
            {
                if (await _schedulerCenterService.AddScheduleJobAsync(quartzNet))
                {
                    return Ok(OperateResult.Success());
                }

                return Ok(OperateResult.Error("执行失败,请重试！"));
            }

            return Ok(OperateResult.Error("已在运行,请勿重复开启！"));
        }

        return Ok(OperateResult.Error(""));
    }

    /// <summary>
    /// 暂停作业
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut]
    [Description("暂停")]
    [Route("pause")]
    public async Task<ActionResult> Pause(long id)
    {
        if (id.IsNullOrEmpty())
        {
            return Error("id cannot be empty");
        }

        var quartzNet = await _quartzNetService.TableWhere(x => x.Id == id).FirstAsync();
        if (quartzNet.IsNull())
        {
            return Error("作业调度不存在");
        }

        var triggerStatus = await _schedulerCenterService.GetTriggerStatus(App.Mapper.MapTo<QuartzNetDto>(quartzNet));
        if (triggerStatus == "运行中")
        {
            //检查任务在内存状态
            var isTrue = await _schedulerCenterService.IsExistScheduleJobAsync(quartzNet);
            if (isTrue && await _schedulerCenterService.PauseJob(quartzNet))
            {
                return Ok(OperateResult.Success());
            }
        }

        return Error("暂停失败,请重试！");
    }

    /// <summary>
    /// 恢复作业
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut]
    [Description("恢复作业")]
    [Route("resume")]
    public async Task<ActionResult> Resume(long id)
    {
        if (id.IsNullOrEmpty())
        {
            return Error("id cannot be empty");
        }

        var quartzNet = await _quartzNetService.TableWhere(x => x.Id == id).FirstAsync();
        if (quartzNet.IsNull())
        {
            return Error("作业调度不存在");
        }

        var triggerStatus = await _schedulerCenterService.GetTriggerStatus(App.Mapper.MapTo<QuartzNetDto>(quartzNet));
        if (triggerStatus == "暂停")
        {
            //检查任务在内存状态
            var isTrue = await _schedulerCenterService.IsExistScheduleJobAsync(quartzNet);
            if (isTrue && await _schedulerCenterService.ResumeJob(quartzNet))
            {
                return Ok(OperateResult.Success());
            }
        }

        return Error("恢复失败,请重试！");
    }


    /// <summary>
    /// 作业调度执行日志
    /// </summary>
    /// <param name="quartzNetLogQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("logs/query")]
    [Description("执行日志")]
    public async Task<ActionResult> QueryLog(QuartzNetLogQueryCriteria quartzNetLogQueryCriteria,
        Pagination pagination)
    {
        var quartzNetLogList = await _quartzNetLogService.QueryAsync(quartzNetLogQueryCriteria, pagination);

        return JsonContent(quartzNetLogList, pagination);
    }

    /// <summary>
    /// 导出作业日志
    /// </summary>
    /// <param name="quartzNetLogQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("导出")]
    [Route("logs/download")]
    public async Task<ActionResult> Download(QuartzNetLogQueryCriteria quartzNetLogQueryCriteria)
    {
        var quartzNetLogExports = await _quartzNetLogService.DownloadAsync(quartzNetLogQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(quartzNetLogExports, out var mimeType, out var fileName);
        return new FileContentResult(data, mimeType)
        {
            FileDownloadName = fileName
        };
    }

    #endregion
}
