using System;
using System.Collections.Generic;
using System.Text;

namespace Kafka.Learning.EventBus
{
    public interface IPollingConsumer
    {
        void StartPolling(string topicName);
        void StopPolling();
    }
}
