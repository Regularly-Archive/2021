using FakeRpc.Core;
using FakeRpc.Core.Client;
using FakeRpc.Core.Mvc;
using FakeRpc.Web.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
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
            var greetProxy = clientFactory.Create<IGreetService>(MessagePackRpcCalls.Factory);
            var reply = await greetProxy.SayHello(new HelloRequest() { Name = "张三" });

            var calcuProxy = clientFactory.Create<ICalculatorService>(MessagePackRpcCalls.Factory);
            var result = await calcuProxy.Calculate(new CalculatorRequest { Num1 = 1, Num2 = 2, Op = "+" });
        }
    }
}
