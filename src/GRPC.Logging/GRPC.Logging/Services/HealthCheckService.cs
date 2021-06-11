using Grpc.Core;
using Grpc.Health.V1;
using Grpc.HealthCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRPC.Logging.Services
{
    public class HealthCheckService : Health.HealthBase
    {
        public override Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
        {
            // TODO: 在这里添加更多的细节
            return Task.FromResult(new HealthCheckResponse() { 
                Status = HealthCheckResponse.Types.ServingStatus.Serving 
            });
        }

        public override async Task Watch(HealthCheckRequest request, IServerStreamWriter<HealthCheckResponse> responseStream, ServerCallContext context)
        {
            // TODO: 在这里添加更多的细节
            await responseStream.WriteAsync(new HealthCheckResponse(){
                Status = HealthCheckResponse.Types.ServingStatus.Serving 
            });
        }
    }
}
