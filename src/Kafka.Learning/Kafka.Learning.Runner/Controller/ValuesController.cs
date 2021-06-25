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
    public class ValuesController : ControllerBase
    {
        private readonly IEventBus _eventBus;
        public ValuesController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        // GET: api/<ValuesController>
        [HttpGet]
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

            _eventBus.Publish(new WriteLogEvent()
            {
                TRANSACTION_ID = Guid.NewGuid().ToString("N"),
                LOG_LEVEL = "DEBUG",
                HOST_NAME = "localhost",
                HOST_IP = "localhost",
                CONTENT = "起风了，唯有努力生存",
                USER_ID = "飞鸿踏雪",
                TTID = "Default",
                APP_NAMESPACE = "ASP.NET Core"
            });
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
