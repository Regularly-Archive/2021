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
        private IConsulClient _consulClient;
        public ConsulServiceDiscovery(IConsulClient consulClient)
        {
            _consulClient = consulClient;
        }

        public override Uri GetService(string serviceName, string serviceGroup)
        {
            var services = AsyncHelper.RunSync<QueryResult<ServiceEntry[]>>(() => _consulClient.Health.Service(serviceName));
            var serviceUrls = services.Response
                .ToList()
                .Where(x => x.Service.Tags.Contains(serviceGroup))
                .Select(x => new Uri($"https://{x.Service.Address}:{x.Service.Port}"))
                .ToList(); ;

            var rand = new Random();
            var index = rand.Next(0, serviceUrls.Count);
            return serviceUrls[index];
        }
    }
}
