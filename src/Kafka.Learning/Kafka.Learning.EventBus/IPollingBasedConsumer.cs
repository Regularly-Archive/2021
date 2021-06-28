using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kafka.Learning.EventBus
{
    public interface IPollingBasedConsumer
    {
        void StartPolling(string topicName);
        void StopPolling();
        void Ack(ConsumeResult<string, byte[]> consumeResult);
    }
}
