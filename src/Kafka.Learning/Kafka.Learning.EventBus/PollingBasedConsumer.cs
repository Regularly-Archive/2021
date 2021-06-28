using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kafka.Learning.EventBus
{
    public class PollingBasedConsumer : IPollingBasedConsumer
    {
        private readonly IConsumer<string, byte[]> _consumer;
        private readonly System.Timers.Timer _pollingTimer;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public Action<ConsumeResult<string, byte[]>> OnConsume;
        public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(30);
        public PollingBasedConsumer(ConsumerConfig consumeConfig)
        {
            _consumer = BuildConsumer<string, byte[]>(consumeConfig, builder => 
            {
                builder.SetKeyDeserializer(Deserializers.Utf8);
                builder.SetValueDeserializer(Deserializers.ByteArray);
                builder.SetErrorHandler((producer, error) => Console.WriteLine("ErrorCode={0},Reason={1},IsFatal={2}", error.Code, error.Reason, error.IsFatal));
            });
            _pollingTimer = new System.Timers.Timer();
            _pollingTimer.Interval = PollingInterval.TotalMilliseconds;
            _pollingTimer.Elapsed += (s, e) =>
            {
                var consumeResult = _consumer.Consume(_cancellationTokenSource.Token);
                if (consumeResult != null)
                    OnConsume?.Invoke(consumeResult);
            };
        }

        public void StartPolling(string topicName)
        {
            _consumer.Subscribe(topicName);
            _pollingTimer.Start();
        }

        public void StopPolling()
        {
            _cancellationTokenSource.Cancel();
            _consumer.Unsubscribe();
            _pollingTimer.Stop();
        }

        private IConsumer<TKey, TValue> BuildConsumer<TKey, TValue>(ConsumerConfig configuration, Action<ConsumerBuilder<TKey, TValue>> configure)
        {
            var consumerBuilder = new ConsumerBuilder<TKey, TValue>(configuration);
            configure?.Invoke(consumerBuilder);
            return consumerBuilder.Build();
        }

        public void Ack(ConsumeResult<string, byte[]> consumeResult)
        {
            _consumer?.Commit(consumeResult);
        }
    }
}
