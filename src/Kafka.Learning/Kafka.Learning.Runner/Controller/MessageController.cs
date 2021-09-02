using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kafka.Learning.EventBus.Runner.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IEventBus _eventBus;
        public MessageController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        // GET: api/Message/Publish
        [HttpGet("Publish")]
        public void Get()
        {
            _eventBus.Publish(new OrderInfoCreateEvent()
            {
                ORDER_ID = "OR001",
            });

            _eventBus.Publish(new OrderInfoCreateEvent()
            {
                ORDER_ID = "OR002",
            });

            for (var i = 0; i < 100; i++)
            {
                _eventBus.Publish(new WriteLogEvent()
                {
                    TRANSACTION_ID = i.ToString(),
                    LOG_LEVEL = "DEBUG",
                    HOST_NAME = "localhost",
                    HOST_IP = "localhost",
                    CONTENT = "起风了，唯有努力生存",
                    USER_ID = "飞鸿踏雪",
                    TTID = "Default",
                    APP_NAMESPACE = "ASP.NET Core"
                }) ;
            }
        }
    }
}
