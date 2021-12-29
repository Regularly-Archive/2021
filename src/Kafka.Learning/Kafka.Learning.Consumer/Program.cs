using Confluent.Kafka;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
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
                BootstrapServers = "192.168.6.23:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                //AutoCommitIntervalMs = 5000,
                EnableAutoCommit = true
            };
            using (var c = new ConsumerBuilder<Null, string>(consumerConfig).Build())
            {
                var cts = new CancellationTokenSource();
                c.Subscribe("daily_event",cts.Token, message =>
                {
                    var userInfo = UserContext.GetUserInfo();
                    Console.WriteLine($"{userInfo.UserName} send message:{message}");
                });
            }

            Console.ReadKey();
        }
    }
}
