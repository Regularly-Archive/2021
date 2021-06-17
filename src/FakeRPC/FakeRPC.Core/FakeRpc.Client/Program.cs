using FakeRpc.Core;
using FakeRpc.Core.Client;
using FakeRpc.Core.Mvc;
using FakeRpc.Web.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace FakeRpc.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddFakeRpcClient<IGreetService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001");
                client.DefaultRequestVersion = new Version(2, 0);
            });

            services.AddFakeRpcClient<ICalculatorService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001");
                client.DefaultRequestVersion = new Version(2, 0);
            });


            var serviceProvider = services.BuildServiceProvider();
            var clientFactory = serviceProvider.GetService<FakeRpcClientFactory>();


            // Client With MessagePack
            var watch = new Stopwatch();
            watch.Start();
            var greetProxy = clientFactory.Create<IGreetService>(MessagePackRpcCalls.Factory);
            var reply = await greetProxy.SayHello(new HelloRequest() { Name = "张三" });
            watch.Stop();
            Console.WriteLine($"MessagePack + HTTP/2 using {watch.ElapsedMilliseconds} ms");

            // Client With Protobuf
            watch = new Stopwatch();
            watch.Start();
            greetProxy = clientFactory.Create<IGreetService>(ProtobufRpcCalls.Factory);
            reply = await greetProxy.SayHello(new HelloRequest() { Name = "张三" });
            watch.Stop();
            Console.WriteLine($"Protobuff + HTTP/2 using {watch.ElapsedMilliseconds} ms");

            // Client With Json
            watch = new Stopwatch();
            watch.Start();
            greetProxy = clientFactory.Create<IGreetService>();
            reply = await greetProxy.SayHello(new HelloRequest() { Name = "张三" });
            watch.Stop();
            Console.WriteLine($"JSON + HTTP/2 using {watch.ElapsedMilliseconds} ms");
        }
    }
}
