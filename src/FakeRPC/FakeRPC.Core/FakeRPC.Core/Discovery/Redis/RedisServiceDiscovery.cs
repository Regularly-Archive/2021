using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using CSRedis;
using Microsoft.Extensions.Logging;
using static CSRedis.CSRedisClient;

namespace FakeRpc.Core.Discovery.Redis
{
    public class RedisServiceDiscovery : BaseServiceDiscovey
    {
        private readonly CSRedisClient _redisClient;
        private readonly RedisServiceDiscoveryOptions _options;
        private readonly ILogger<RedisServiceDiscovery> _logger;

        public RedisServiceDiscovery(RedisServiceDiscoveryOptions options, ILogger<RedisServiceDiscovery> logger)
        {
            _options = options;
            _redisClient = new CSRedisClient(options.RedisUrl);
            RedisHelper.Initialization(_redisClient);
            _redisClient.Subscribe((_options.RegisterEventTopic, OnServiceRegister));
            _redisClient.Subscribe((_options.RegisterEventTopic, OnServiceUnregister));
            _logger = logger;
        }

        public override IEnumerable<Uri> GetService(string serviceName, string serviceGroup)
        {
            var serviceDiscoveryKey = GetServiceDiscoveryKey(serviceName);
            var serviceNodes = _redisClient.SMembers<ServiceRegistration>(serviceDiscoveryKey);
            serviceNodes = serviceNodes.Where(x => x.ServiceGroup == serviceGroup).ToArray();
            if (serviceNodes == null)
                throw new ArgumentException($"Service {serviceGroup}.{serviceName} can't be resolved.");

            _logger.LogInformation($"Discovery {serviceNodes.Count()} instances for {serviceName} ...");
            return serviceNodes.Select(x => x.ServiceUri);
        }

        private void OnServiceRegister(SubscribeMessageEventArgs args)
        {

        }

        private void OnServiceUnregister(SubscribeMessageEventArgs args)
        {

        }
    }
}
