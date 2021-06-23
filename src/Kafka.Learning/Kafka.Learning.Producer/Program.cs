using Microsoft.Extensions.DependencyInjection;
using Confluent.Kafka;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;

namespace Kafka.Learning.Producer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var producerConfig = new ProducerConfig { BootstrapServers = "192.168.50.162:9092" };
            using (var p = new ProducerBuilder<Null, string>(producerConfig).Build())
            {
                try
                {
                    for (var i = 1; i <= 100; i++)
                    {
                        var student = new { Id = i.ToString("000"), Name = $"Student{i}" };
                        var result = await p.ProduceAsync("test", new Message<Null, string> { Value = JsonConvert.SerializeObject(student) });
                        Console.WriteLine($"Delivered '{result.Value}'");
                    }

                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }

            Console.WriteLine("Send Done!");
            Console.ReadKey();
        }
    }
}
