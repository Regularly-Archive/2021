using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kafka.Learning.EventBus
{
    public class KafkaEventBus : IEventBus
    {
        private IProducer<string, byte[]> _producer;
        private ILogger<KafkaEventBus> _logger;
        private ConsumerConfig _consumeConfig;
        private IEventBusSubscriptionManager _subscriptionManager;
        private IServiceProvider _serviceProvider;
        public KafkaEventBus(
            ProducerConfig producerConfig,
            ConsumerConfig consumeConfig,
            ILogger<KafkaEventBus> logger,
            IEventBusSubscriptionManager subscriptionManager,
            IServiceProvider serviceProvider
        )
        {
            _logger = logger;
            _consumeConfig = consumeConfig;
            _serviceProvider = serviceProvider;
            _subscriptionManager = subscriptionManager;
            _subscriptionManager.OnSubscribe += async (s, e) =>
            {
                await MakeSureTopicExists(e.EvenType.FullName);
                var consumer = new PollingBasedConsumer(_consumeConfig);
                var eventName = e.EvenType.FullName;
                consumer.OnConsume = async consumeResult =>
                {
                    await Task.WhenAll(ProcessEvent(consumeResult));
                    consumer.Ack();
                };
                consumer.StartPolling(eventName);
            };
            _producer = BuildProducer<string, byte[]>(producerConfig, builder =>
             {
                 builder.SetKeySerializer(Serializers.Utf8);
                 builder.SetValueSerializer(Serializers.ByteArray);
                 builder.SetErrorHandler((producer, error) => _logger.LogError("ErrorCode={0},Reason={1},IsFatal={2}", error.Code, error.Reason, error.IsFatal));
             });
        }

        public void Publish<TEvent>(TEvent @event) where TEvent : EventBase
        {
            var eventName = typeof(TEvent).FullName;
            var eventBody = JsonConvert.SerializeObject(@event);
            MakeSureTopicExists(eventName).Wait();
            _producer.Produce(eventName, new Message<string, byte[]> { Value = Encoding.UTF8.GetBytes(eventBody) }, deliveryReport =>
            {
                _logger.LogInformation($"Delivery message \"{eventBody}\" to topic \"{deliveryReport.Topic}\" {deliveryReport.Status}.");
            });
        }

        public void Subscribe<T, TH>()
            where T : EventBase
            where TH : IEventHandler<T>
        {
            _subscriptionManager.Subscribe<T, TH>();
        }

        public void Unsubscribe<T, TH>()
            where T : EventBase
            where TH : IEventHandler<T>
        {
            _subscriptionManager.Unsubscribe<T, TH>();
        }

        private IProducer<TKey, TValue> BuildProducer<TKey, TValue>(ProducerConfig configuration, Action<ProducerBuilder<TKey, TValue>> configure)
        {
            var producerBuilder = new ProducerBuilder<TKey, TValue>(configuration);
            configure?.Invoke(producerBuilder);
            return producerBuilder.Build();
        }

        private IConsumer<TKey, TValue> BuildConsumer<TKey, TValue>(ConsumerConfig configuration, Action<ConsumerBuilder<TKey, TValue>> configure)
        {
            var consumerBuilder = new ConsumerBuilder<TKey, TValue>(configuration);
            configure?.Invoke(consumerBuilder);
            return consumerBuilder.Build();
        }

        private IEnumerable<Task> ProcessEvent(ConsumeResult<string,byte[]> consumeResult)
        {
            var eventName = consumeResult.Topic;
            var message = Encoding.UTF8.GetString(consumeResult.Message.Value);
            if (_subscriptionManager.IsEventSubscribed(eventName))
            {
                var policy = BuildProcessEventPolicy();
                using (var serviceScope = _serviceProvider.CreateScope())
                {
                    foreach (var handlerType in _subscriptionManager.GetHandlersForEvent(eventName))
                    {
                        var handler = serviceScope.ServiceProvider.GetRequiredService(handlerType);
                        if (handler == null) continue;

                        var eventType = _subscriptionManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);

                        _logger.LogInformation($"Process event \"{eventName}\" with \"{handler.GetType().Name}\"...");
                        yield return (Task)policy.Execute(() => concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent }));
                    }
                }
            }
        }

        private PolicyWrap BuildProcessEventPolicy()
        {
            //重试策略
            var _retryCount = 5;
            var retryPolicy = RetryPolicy
                .Handle<Exception>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogInformation($"Process event fails due to \"{ex.InnerException?.Message}\" and  it will re-try after {time.TotalSeconds}s");
                });

            //超时策略
            var timeoutPolicy = TimeoutPolicy.Timeout(30);

            //组合策略
            return retryPolicy.Wrap(timeoutPolicy);
        }

        private async Task MakeSureTopicExists(string topicName)
        {
            var adminClient = new AdminClientBuilder(new AdminClientConfig() { BootstrapServers = "192.168.50.162:9092" }).Build();
            var matadata = adminClient.GetMetadata(topicName, TimeSpan.FromSeconds(10));
            if (matadata.Topics == null || matadata.Topics.Count == 0)
            {
                await adminClient.CreateTopicsAsync(new List<TopicSpecification>
                {
                    new TopicSpecification() { Name = topicName, NumPartitions = 1, ReplicationFactor = 1 }
                });
            }

            await Task.CompletedTask;
        }


    }
}
