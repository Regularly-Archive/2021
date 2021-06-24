using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Kafka.Learning.EventBus
{
    public class PollingConsumer : IPollingConsumer
    {
        private IConsumer<string, byte[]> _consumer;
        private System.Timers.Timer _pollingTimer;
        private CancellationTokenSource _cancellationTokenSource;
        public Action<ConsumeResult<string, byte[]>> OnConsume;
        public PollingConsumer(ConsumerConfig consumeConfig)
        {
            _consumer = BuildConsumer<string, byte[]>(consumeConfig, builder => 
            {
                builder.SetKeyDeserializer(Deserializers.Utf8);
                builder.SetValueDeserializer(Deserializers.ByteArray);
                builder.SetErrorHandler((producer, error) => Console.WriteLine("ErrorCode={0},Reason={1},IsFatal={2}", error.Code, error.Reason, error.IsFatal));
            });
            _pollingTimer = new System.Timers.Timer();
            _pollingTimer.Interval = 200;
            _cancellationTokenSource = new CancellationTokenSource();
            _pollingTimer.Elapsed += (s, e) =>
            {
                var consumeResult = _consumer.Consume(200);
                if (consumeResult != null)
                {
                    OnConsume?.Invoke(consumeResult);
                }
            };
        }

        public void StartPolling(string topicName)
        {
            _consumer.Subscribe(topicName);
            _pollingTimer.Start();
        }

        public void StopPolling()
        {
            _consumer.Unsubscribe();
            _pollingTimer.Stop();
        }

        private IConsumer<TKey, TValue> BuildConsumer<TKey, TValue>(ConsumerConfig configuration, Action<ConsumerBuilder<TKey, TValue>> configure)
        {
            var consumerBuilder = new ConsumerBuilder<TKey, TValue>(configuration);
            configure?.Invoke(consumerBuilder);
            return consumerBuilder.Build();
        }
    }
}
