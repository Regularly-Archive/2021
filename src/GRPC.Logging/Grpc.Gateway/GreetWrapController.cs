using GRPC.Logging;
using Microsoft.AspNetCore.Mvc;
using System;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Grpc.Gateway
{
    public class GreetWrapController : Controller
    {
        private IServiceProvider _serviceProvider;
        public GreetWrapController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public async Task<ActionResult> SayHello(HelloRequestDTO requestDTO)
        {
            var request = requestDTO.Adapt<HelloRequest>();
            var client = _serviceProvider.GetService<Greeter.GreeterClient>();
            var replay = await client.SayHelloAsync(request);
            return new JsonResult(replay);
        }
    }

}