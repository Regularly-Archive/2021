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
                client.BaseAddress = new Uri("http://localhost:5000");
            });

            var serviceProvider = services.BuildServiceProvider();
            var clientFactory = serviceProvider.GetService<FakeRpcClientFactory>();
            var clientProxy = clientFactory.Create<IGreetService>(new Uri("http://localhost:5000"), MessagePackRpcCalls.Factory);
            var reply = await clientProxy.SayHello(new HelloRequest() { Name = "张三" });
        }
    }
}
