using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GrpcJwt
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GreeterService(
            ILogger<GreeterService> logger,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            var userName = _httpContextAccessor.HttpContext.User?.Identity.Name;
            return Task.FromResult(new HelloReply { Message = $"Hello {userName}"});
        }
    }
}
