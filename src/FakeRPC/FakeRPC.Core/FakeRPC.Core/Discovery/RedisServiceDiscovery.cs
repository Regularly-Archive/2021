using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using CSRedis;
using Microsoft.Extensions.Logging;

namespace FakeRpc.Core.Discovery
{
    public class RedisServiceDiscovery : BaseServiceDiscovey
    {
        private readonly CSRedisClient _redisClient;

        private readonly ILogger<RedisServiceDiscovery> _logger;

        public RedisServiceDiscovery(CSRedisClient redisClient, ILogger<RedisServiceDiscovery> logger)
        {
            _redisClient = redisClient;
            _logger = logger;
        }

        public override Uri GetService(string serviceName, string serviceGroup)
        {
            var serviceDiscoveryKey = GetServiceDiscoveryKey(serviceName);
            var serviceNodes = _redisClient.SMembers<ServiceRegistration>(serviceDiscoveryKey);
            var serviceNode = serviceNodes.FirstOrDefault(x => x.ServiceGroup == serviceGroup);
            if (serviceNode == null)
                throw new ArgumentException($"Service {serviceGroup}.{serviceName} can't be resolved.");

            _logger.LogInformation($"Discovery {serviceGroup}.{serviceName} {serviceNode.ServiceUri} ...");
            return serviceNode.ServiceUri;
        }
    }
}
