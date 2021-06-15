using FakeRpc.Core;
using FakeRpc.Core.Mvc;
using FakeRpc.Web.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FakeRpc.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clientFactory = new FakeRpcClientFactory();
            var clientProxy = clientFactory.Create<IGreetService>(new Uri("http://localhost:5000"));
            var reply = await clientProxy.SayHello(new HelloRequest() { Name = "张三" });
        }
    }
}
