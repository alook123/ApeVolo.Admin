using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.ActionExtension.Sign;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.Attributes;
using Ape.Volo.Common.Model;
using Ape.Volo.Core.Caches;
using Ape.Volo.IBusiness.Test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.Test;

/// <summary>
/// 测试
/// </summary>
[Area("测试")]
[Route("/api/test", Order = 999)]
public class TestController : BaseApiController
{
    //private readonly IEmailScheduleTask _emailScheduleTask;
    //
    // //private readonly IEventBus _eventBus;
    // private readonly IRedisCacheService _redisCacheService;
    private readonly ITestOrderService _testOrderService;
    private ICache _cache;

    // private readonly IUserService _userService;
    // private readonly IRoleService _roleService;
    // private readonly IBrowserDetector _browserDetector;

    public TestController(ITestOrderService testOrderService, ICache cache)
    {
        _testOrderService = testOrderService;
        _cache = cache;
        //_eventBus = eventBus;
    }


    // [HttpGet]
    // [AllowAnonymous]
    // public async Task<ActionResult> TestSecret()
    // {
    //     //await _emailScheduleTask.ExecuteAsync();
    //     await Task.CompletedTask;
    //     return Success();
    // }


    // [HttpPost]
    // [AllowAnonymous]
    // public async Task<ActionResult> AddApeVolo()
    // {
    //     try
    //     {
    //         await _testApeVoloService.CreateAsync(new TestApeVolo
    //         {
    //             Label = "test",
    //             Content = "test",
    //             Sort = 1
    //         });
    //     }
    //     catch
    //     {
    //         // ignored
    //     }
    //
    //     return Success();
    // }

    /// <summary>
    /// 签名测试
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [VerifySignature]
    public async Task<ActionResult> TestSign()
    {
        try
        {
            await Task.CompletedTask;
        }
        catch
        {
            // ignored
        }

        return Ok(OperateResult.Success());
    }

    // [HttpGet]
    // [AllowAnonymous]
    // public async Task<ActionResult> TestRedisMq()
    // {
    //     try
    //     {
    //         await _cache.GetDatabase().ListLeftPushAsync(MqTopicNameKey.MailboxQueue, "123456789");
    //     }
    //     catch
    //     {
    //         // ignored
    //     }
    //
    //     return Success();
    // }


    /*[HttpGet]
    [AllowAnonymous]
    public ActionResult<object> EventBusTry()
    {
        try
        {
            var userId = 163519427764300;
            for (var i = 1; i <= 1000; i++)
            {
                var eventMessage = new UserQueryIntegrationEvent(userId, i);
                _eventBus.Publish(eventMessage);
            }
        }
        catch
        {
            // ignored
        }

        return Success();
    }*/

    [HttpGet]
    [Route("SearchOrder")]
    [Description("Sys.Query")]
    [NotAudit]
    public async Task<ActionResult> SearchOrder()
    {
        // await _testOrderService.AddEntityAsync(new TestOrder()
        // {
        //     Id = IdHelper.GetLongId(),
        //     OrderNo = "1001",
        //     GoodsName = "iphone 16",
        //     Qty = 1,
        //     Price = 5000
        // });
        var list = await _testOrderService.Table.ToListAsync();
        return JsonContent(list);
    }
}
