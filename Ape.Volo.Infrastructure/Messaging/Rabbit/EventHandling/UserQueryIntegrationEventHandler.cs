﻿using Ape.Volo.Common.Helper;
using Ape.Volo.EventBus.Abstractions;
using Ape.Volo.IBusiness.Permission;
using Ape.Volo.Infrastructure.Messaging.Rabbit.Events;
using Microsoft.Extensions.Logging;

namespace Ape.Volo.Infrastructure.Messaging.Rabbit.EventHandling
{
    /// <summary>
    /// 测试rabbitmq事件总线
    /// </summary>
    public class UserQueryIntegrationEventHandler : IIntegrationEventHandler<UserQueryIntegrationEvent>
    {
        private readonly ILogger<UserQueryIntegrationEventHandler> _logger;
        private readonly IUserService _userService;

        public UserQueryIntegrationEventHandler(IUserService userService,
            ILogger<UserQueryIntegrationEventHandler> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="event"></param>
        public async Task Handle(UserQueryIntegrationEvent @event)
        {
            _logger.LogInformation($"----- Handling integration event: {@event.Id} at {@event}");
            ConsoleHelper.WriteLine($"----- Handling integration event: {@event.Id} at ApeVolo - ({@event})");
            await _userService.QueryByIdAsync(@event.UserId);
        }
    }
}
