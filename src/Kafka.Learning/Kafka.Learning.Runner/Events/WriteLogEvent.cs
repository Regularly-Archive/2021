using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kafka.Learning.EventBus
{
    [Serializable]
    public class WriteLogEvent : EventBase
    {
        public string TRANSACTION_ID { get; set; }
        public string LOG_LEVEL { get; set; }
        public string HOST_NAME { get; set; }
        public string HOST_IP { get; set; }
        public string CONTENT { get; set; }
        public string USER_ID { get; set; }
        public string TTID { get; set; }
        public string APP_NAMESPACE { get; set; }
    }
}
