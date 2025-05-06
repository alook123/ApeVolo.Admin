﻿using System.Threading.Tasks;
using Ape.Volo.Entity.Core.System.QuartzNet;
using Ape.Volo.ViewModel.Core.System.QuartzNet;

namespace Ape.Volo.TaskService.service;

/// <summary>
/// 作业调度接口
/// </summary>
public interface ISchedulerCenterService
{
    /// <summary>
    /// 开启任务
    /// </summary>
    /// <returns></returns>
    Task<bool> StartScheduleAsync();

    /// <summary>
    /// 停止任务
    /// </summary>
    /// <returns></returns>
    Task<bool> ShutdownScheduleAsync();

    /// <summary>
    /// 添加任务
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    Task<bool> AddScheduleJobAsync(QuartzNet taskQuartz);

    /// <summary>
    /// 停止任务
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    Task<bool> DeleteScheduleJobAsync(QuartzNet taskQuartz);

    /// <summary>
    /// 暂停任务
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    Task<bool> PauseJob(QuartzNet taskQuartz);

    /// <summary>
    /// 恢复任务
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    Task<bool> ResumeJob(QuartzNet taskQuartz);

    /// <summary>
    /// 检测任务是否存在
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    Task<bool> IsExistScheduleJobAsync(QuartzNet taskQuartz);


    /// <summary>
    /// 获取任务触发器状态
    /// </summary>
    /// <param name="taskQuartzVo"></param>
    /// <returns></returns>
    Task<string> GetTriggerStatus(QuartzNetVo taskQuartzVo);
}
