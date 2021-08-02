using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using FakeRpc.Core;
using FakeRpc.Core.Client;
using FakeRpc.Core.Discovery;
using FakeRpc.Core.Discovery.Consul;
using FakeRpc.Core.LoadBalance;
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
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<TestContext>();
            Console.ReadKey();
        }
    }

    public class TestContext
    {
        public IServiceProvider InitIoc()
        {
            var services = new ServiceCollection();

            var builder = new FakeRpcClientBuilder(services);

            builder.AddRpcClient<IGreetService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001");
                client.DefaultRequestVersion = new Version(1, 0);
            });

            builder.AddRpcClient<ICalculatorService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001");
                client.DefaultRequestVersion = new Version(1, 0);
            });

            builder.AddRpcCallsFactory(MessagePackRpcCalls.Factory);
            builder.EnableLoadBalance<RandomLoadBalanceStrategy>();
            builder.EnableConsulServiceDiscovery(new ConsulServiceDiscoveryOptions()
            {
                BaseUrl = "http://localhost:8500",
                UseHttps = true
            });

            return services.BuildServiceProvider();
        }

        [Benchmark]
        public async Task RunMessagePack()
        {
            var serviceProvider = InitIoc();
            var _clientFactory = serviceProvider.GetService<FakeRpcClientFactory>();
            var greetProxy = _clientFactory.Create<IGreetService>(MessagePackRpcCalls.Factory);
            var reply = await greetProxy.SayHello(new HelloRequest() { Name = "张三" });
            reply = await greetProxy.SayWho();
            var calculatorProxy = _clientFactory.Create<ICalculatorService>(MessagePackRpcCalls.Factory);
            var result = calculatorProxy.Random();
        }

        [Benchmark]
        public async Task RunProtobuf()
        {
            var serviceProvider = InitIoc();
            var _clientFactory = serviceProvider.GetService<FakeRpcClientFactory>();
            var greetProxy = _clientFactory.Create<IGreetService>(ProtobufRpcCalls.Factory);
            var reply = await greetProxy.SayHello(new HelloRequest() { Name = "张三" });
            reply = await greetProxy.SayWho();
            var calculatorProxy = _clientFactory.Create<ICalculatorService>(ProtobufRpcCalls.Factory);
            var result = calculatorProxy.Random();
        }

        [Benchmark]
        public async Task RunJson()
        {
            var serviceProvider = InitIoc();
            var _clientFactory = serviceProvider.GetService<FakeRpcClientFactory>();
            var greetProxy = _clientFactory.Create<IGreetService>(DefaultFakeRpcCalls.Factory);
            var reply = await greetProxy.SayHello(new HelloRequest() { Name = "张三" });
            reply = await greetProxy.SayWho();
            var calculatorProxy = _clientFactory.Create<ICalculatorService>(DefaultFakeRpcCalls.Factory);
            var result = calculatorProxy.Random();
        }
    }
}
