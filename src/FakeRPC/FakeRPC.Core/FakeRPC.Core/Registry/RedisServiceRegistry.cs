using CSRedis;
using FakeRpc.Core.Discovery;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeRpc.Core.Registry
{
    public class RedisServiceRegistry : BaseServiceRegistry
    {
        private readonly CSRedisClient _redisClient;
        private readonly ILogger<RedisServiceRegistry> _logger;

        public RedisServiceRegistry(CSRedisClient redisClient, ILogger<RedisServiceRegistry> logger)
        {
            _redisClient = redisClient;
            _logger = logger;
        }

        public override void Register(ServiceRegistration serviceRegistration)
        {
            var serviceName = serviceRegistration.ServiceName;
            var serviceGroup = serviceRegistration.ServiceGroup;
            var registryKey = GetServiceRegistryKey(serviceName);
            _logger.LogInformation($"Register {serviceGroup}.{serviceName} {serviceRegistration.ServiceUri} ...");
            var serviceNodes = _redisClient.SMembers<ServiceRegistration>(registryKey)?.ToList();
            if (!serviceNodes.Any(x => x.ServiceUri == serviceRegistration.ServiceUri && x.ServiceGroup == serviceRegistration.ServiceGroup))
                _redisClient.SAdd(registryKey, serviceRegistration);
        }

        public override void Unregister(ServiceRegistration serviceRegistration)
        {
            var serviceName = serviceRegistration.ServiceName;
            var serviceGroup = serviceRegistration.ServiceGroup;
            var registryKey = GetServiceRegistryKey(serviceName);
            _logger.LogInformation($"Unregister {serviceGroup}.{serviceName} {serviceRegistration.ServiceUri} ...");
            var serviceNodes = _redisClient.SMembers<ServiceRegistration>(registryKey)?.ToList();
            var serviceNode = serviceNodes.FirstOrDefault(x => x.ServiceUri == serviceRegistration.ServiceUri && x.ServiceGroup == serviceRegistration.ServiceGroup);
            if (serviceNode != null)
                _redisClient.SRem(registryKey, serviceRegistration);
        }
    }
}
