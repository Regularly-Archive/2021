using Kafka.Learning.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kafka.Learning.EventBus
{
    public class EventBusSubscriptionEventArgs
    {
        public Type EvenType { get; set; }
        public Type HandlerType { get; set; }
    }
}
