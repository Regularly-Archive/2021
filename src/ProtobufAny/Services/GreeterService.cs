using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using ProtobufAny.Database;
using ProtobufAny.Extensions;

namespace ProtobufAny
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly QueryService _queryService;
        public GreeterService(
            ILogger<GreeterService> logger,
            QueryService queryService)
        {
            _logger = logger;
            _queryService = queryService;
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
            }
            else if (request.Data.Is(Bar.Descriptor))
            {
                var bar = request.Data.ToObject<Bar>();
                return Task.FromResult(new AnyResponse() { Data = bar.ToAny() });
            }

            return base.Echo(request, context);
        }

        public override Task<MyAny> Ping(MyAny request, ServerCallContext context)
        {
            if (request.Is<Foo>())
            {
                var foo = request.Unpack<Foo>();
                return Task.FromResult(foo.Pack());
            }
            else if (request.Is<Bar>())
            {
                var bar = request.Unpack<Bar>();
                return Task.FromResult(bar.Pack());
            }

            return base.Ping(request, context);
        }

        public override Task<QueryReply> Query(QueryRequest request, ServerCallContext context)
        {
            var inputType = Type.GetType(request.InputType);
            var outputType = Type.GetType(request.OutputType);
            if (inputType != null && outputType != null)
            {
                var queryMethod = _queryService.GetType().GetMethod("Query").MakeGenericMethod(inputType, outputType);
                QueryReply queryResult = (QueryReply)queryMethod.Invoke(_queryService, new object[] { new DynamicSearch.Core.SearchParameters()});
                return Task.FromResult(queryResult);
            }

            return Task.FromResult(new QueryReply());
        }
    }
}
