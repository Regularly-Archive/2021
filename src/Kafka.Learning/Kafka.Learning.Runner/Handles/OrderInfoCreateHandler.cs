using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kafka.Learning.EventBus
{
    public class OrderInfoCreateHandler : IEventHandler<OrderInfoCreateEvent>
    {
        private ILogger<OrderInfoCreateHandler> _logger;

        public OrderInfoCreateHandler(ILogger<OrderInfoCreateHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(OrderInfoCreateEvent @event)
        {
            _logger.LogInformation($"发货订单{@event.ORDER_ID}已创建");
            return Task.CompletedTask;
        }
    }
}
