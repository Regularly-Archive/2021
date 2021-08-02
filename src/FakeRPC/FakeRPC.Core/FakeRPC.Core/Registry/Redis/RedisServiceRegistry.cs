using CSRedis;
using FakeRpc.Core.Discovery;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeRpc.Core.Registry.Redis
{
    public class RedisServiceRegistry : BaseServiceRegistry
    {
        private readonly CSRedisClient _redisClient;
        private readonly ILogger<RedisServiceRegistry> _logger;
        private readonly string _registerEventTopic = "evt_service_register";
        private readonly string _unregisterEventTopic = "evt_service_unregister";

        public RedisServiceRegistry(CSRedisClient redisClient, ILogger<RedisServiceRegistry> logger)
        {
            _redisClient = redisClient;
            _logger = logger;
        }

        public override void Register(ServiceRegistration serviceRegistration)
        {
            var serviceName = serviceRegistration.ServiceName;
            var serviceGroup = serviceRegistration.ServiceGroup;
            var registryKey = GetServiceRegistryKey(serviceGroup,serviceName);
            _logger.LogInformation($"Register {serviceGroup}.{serviceName} {serviceRegistration.ServiceUri} ...");
            var serviceNodes = _redisClient.SMembers<ServiceRegistration>(registryKey)?.ToList();
            if (!serviceNodes.Any(x => x.ServiceUri == serviceRegistration.ServiceUri && x.ServiceGroup == serviceRegistration.ServiceGroup))
            {
                _redisClient.SAdd(registryKey, serviceRegistration);
                Publish(_registerEventTopic, new { Key = registryKey, Value = serviceRegistration });
            }

        }

        public override void Unregister(ServiceRegistration serviceRegistration)
        {
            var serviceName = serviceRegistration.ServiceName;
            var serviceGroup = serviceRegistration.ServiceGroup;
            var registryKey = GetServiceRegistryKey(serviceGroup,serviceName);
            _logger.LogInformation($"Unregister {serviceGroup}.{serviceName} {serviceRegistration.ServiceUri} ...");
            var serviceNodes = _redisClient.SMembers<ServiceRegistration>(registryKey)?.ToList();
            var serviceNode = serviceNodes.FirstOrDefault(x => x.ServiceUri == serviceRegistration.ServiceUri && x.ServiceGroup == serviceRegistration.ServiceGroup);
            if (serviceNode != null)
            {
                _redisClient.SRem(registryKey, serviceRegistration);
                Publish(_unregisterEventTopic, new { Key = registryKey, Value = serviceNode });
            }
        }

        private void Publish(string topic, string message)
        {
            _redisClient.Publish(topic, message);
        }

        private void Publish<TMessage>(string topic, TMessage message)
        {
            _redisClient.Publish(topic, JsonConvert.SerializeObject(message));
        }
    }
}
