using Microsoft.Extensions.DependencyInjection;
using Confluent.Kafka;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using System.Text;

namespace Kafka.Learning.Producer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var producerConfig = new ProducerConfig { BootstrapServers = "192.168.6.23:9092" };
            using (var p = new ProducerBuilder<Null, string>(producerConfig).Build())
            {
                try
                {
                    var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoi566h55CG5ZGYIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJleHAiOjE2NDA4MzA2ODUsImlzcyI6IkdycGNKd3QiLCJhdWQiOiJHcnBjSnd0In0.FhjrJ0c5rQ2gwEp49CbjUS_swEWC2tWWpa218jx35jg";
                    var document = new { Id = "001", Name = "张三", Address = "北京市朝阳区", Event = "喝水未遂" };
                    var message = new Message<Null, string> { Value = JsonConvert.SerializeObject(document) };
                    message.Headers = new Headers();
                    message.Headers.Add("Authorization", Encoding.UTF8.GetBytes($"Bearer {token}"));
                    var result = await p.ProduceAsync("daily_event", message);
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
