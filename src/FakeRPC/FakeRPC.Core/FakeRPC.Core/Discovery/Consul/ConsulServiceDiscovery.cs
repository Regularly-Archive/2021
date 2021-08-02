using Consul;
using FakeRpc.Core.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FakeRpc.Core.Discovery.Consul
{
    public class ConsulServiceDiscovery : BaseServiceDiscovey
    {
        private readonly IConsulClient _consulClient;
        private readonly ConsulServiceDiscoveryOptions _options;
        public ConsulServiceDiscovery(IConsulClient consulClient, ConsulServiceDiscoveryOptions options)
        {
            _consulClient = consulClient;
            _options = options;
        }

        public override IEnumerable<Uri> GetService(string serviceName, string serviceGroup)
        {
            var schema = _options.UseHttps ? "https" : "http";
            var services = AsyncHelper.RunSync<QueryResult<ServiceEntry[]>>(() => _consulClient.Health.Service(serviceName));
            var serviceUrls = services.Response
                .ToList()
                .Where(x => x.Service.Tags.Contains(serviceGroup))
                .Select(x => new Uri($"{schema}://{x.Service.Address}:{x.Service.Port}"))
                .ToList();

            return serviceUrls;
        }
    }
}
