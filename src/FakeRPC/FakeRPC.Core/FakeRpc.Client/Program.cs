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

            var builder = new FakeRpcClientBuilder(services);

            builder.AddRpcClient<IGreetService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001");
                client.DefaultRequestVersion = new Version(2, 0);
            });

            builder.AddRpcClient<ICalculatorService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001");
                client.DefaultRequestVersion = new Version(2, 0);
            });

            //services.AddFakeRpcClient<IGreetService>(new ServiceDiscoveryOptions() 
            //{ 
            //    DiscoveryServer = "localhost:6379",
            //    ServiceNamespace = typeof(GreetService).Namespace
            //});
            //services.AddFakeRpcClient<ICalculatorService>(new ServiceDiscoveryOptions() 
            //{ 
            //    DiscoveryServer = "localhost:6379",
            //    ServiceNamespace = typeof(CalculatorService).Namespace
            ///});

           builder.AddRpcCallsFactory(MessagePackRpcCalls.Factory);


            var serviceProvider = services.BuildServiceProvider();
            var clientFactory = serviceProvider.GetService<FakeRpcClientFactory>();


            // Client With MessagePack
            var watch = new Stopwatch();
            watch.Start();
            var greetProxy = clientFactory.Create<IGreetService>(MessagePackRpcCalls.Factory);
            var reply = await greetProxy.SayHello(new HelloRequest() { Name = "张三" });
            reply = await greetProxy.SayWho();
            var calculatorProxy = clientFactory.Create<ICalculatorService>(MessagePackRpcCalls.Factory);
            var result = calculatorProxy.Random();
            watch.Stop();
            Console.WriteLine($"MessagePack + HTTP/2 using {watch.ElapsedMilliseconds} ms");

            // Client With Protobuf
            watch = new Stopwatch();
            watch.Start();
            greetProxy = clientFactory.Create<IGreetService>(ProtobufRpcCalls.Factory);
            reply = await greetProxy.SayHello(new HelloRequest() { Name = "张三" });
            reply = await greetProxy.SayWho();
            calculatorProxy = clientFactory.Create<ICalculatorService>(ProtobufRpcCalls.Factory);
            result = calculatorProxy.Random();
            watch.Stop();
            Console.WriteLine($"Protobuff + HTTP/2 using {watch.ElapsedMilliseconds} ms");

            // Client With Json
            watch = new Stopwatch();
            watch.Start();
            greetProxy = clientFactory.Create<IGreetService>(DefaultFakeRpcCalls.Factory);
            reply = await greetProxy.SayHello(new HelloRequest() { Name = "张三" });
            reply = await greetProxy.SayWho();
            calculatorProxy = clientFactory.Create<ICalculatorService>(DefaultFakeRpcCalls.Factory);
            result = calculatorProxy.Random();
            watch.Stop();
            Console.WriteLine($"JSON + HTTP/2 using {watch.ElapsedMilliseconds} ms");
        }
    }
}
