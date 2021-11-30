using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using ProtobufAny.Extensions;

namespace ProtobufAny
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override Task<AnyResponse> Echo(AnyRequest request, ServerCallContext context)
        {
            if (request.Data.Is(Foo.Descriptor))
            {
                var foo = request.Data.ToObject<Foo>();
                return Task.FromResult(new AnyResponse() { Data = foo.ToAny() });
            } else if (request.Data.Is(Bar.Descriptor))
            {
                var bar = request.Data.ToObject<Bar>();
                return Task.FromResult(new AnyResponse() { Data = bar.ToAny() });
            }

            return base.Echo(request, context);
        }
    }
}
