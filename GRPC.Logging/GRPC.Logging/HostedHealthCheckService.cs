using Grpc.Health.V1;
using Grpc.Net.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GRPC.Logging
{
    public class HostedHealthCheckService : IHostedService
    {
        private Timer _timer = null;
        private readonly ILogger<HostedHealthCheckService> _logger;

        public HostedHealthCheckService(ILogger<HostedHealthCheckService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {;
            _logger.LogInformation($"{nameof(HostedHealthCheckService)} start running....");
            _timer = new Timer(DoCheck, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(HostedHealthCheckService)} stop running....");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void DoCheck(object state)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001"); ;
            var client = new Health.HealthClient(channel);
            client.Check(new HealthCheckRequest() { Service = "https://localhost:5001" });
        }
    }
}
