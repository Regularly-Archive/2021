using Confluent.Kafka;
using System;
using System.Threading;

namespace Kafka.Learning.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var consumerConfig = new ConsumerConfig
            {
                GroupId = "test-consumer-group",
                BootstrapServers = "192.168.50.162:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                AutoCommitIntervalMs = 5000,
                EnableAutoCommit = true
            };
            using (var c = new ConsumerBuilder<Null, string>(consumerConfig).Build())
            {
                c.Subscribe("test");

                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true;
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var consumeResult = c.Consume(cts.Token);
                            Console.WriteLine($"Consumed message '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'.");
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
            }

            Console.WriteLine("Send Done!");
            Console.ReadKey();
        }
    }
}
