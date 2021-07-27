using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using CSRedis;

namespace FakeRpc.Core.Discovery
{
    public class RpcDiscoveryService : IRpcDiscoveryService
    {
        private CSRedisClient _redisClient;

        public RpcDiscoveryService(CSRedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        public Task DisableService<TService>()
        {
            var serviceName = typeof(TService).GetServiceName();
            return DisableService(serviceName);
        }

        public Task DisableService(string serviceName)
        {
            var clusters = _redisClient.SMembers<RpcServiceEntry>(serviceName);
            if (clusters != null && clusters.Any())
                clusters.ToList().ForEach(x => x.IsEnable = false);

            _redisClient.SPop(serviceName);
            _redisClient.SAdd<RpcServiceEntry>(serviceName, clusters);

            throw new ArgumentException($"Service {serviceName} can't be resolved.");
        }

        public Uri GetService<TService>()
        {
            var serviceName = typeof(TService).GetServiceName();
            return GetService(serviceName);
        }

        public Uri GetService(string serviceName)
        {
            var clusters = _redisClient.SMembers<RpcServiceEntry>(serviceName);
            if (clusters != null && clusters.Any())
            {
                var rnd = new Random();
                var index = rnd.Next(0, clusters.Length);
                return clusters[index].ServiceUri;
            }

            throw new ArgumentException($"Service {serviceName} can't be resolved.");
        }

        public Task RegisterService<TService>(Uri serviceUri)
        {
            var serviceName = typeof(TService).GetServiceName();
            return RegisterService(serviceName, serviceUri);
        }

        public Task RegisterService(string serviceName, Uri serviceUri)
        {
            var serviceEntry = new RpcServiceEntry()
            {
                ServiceId = Guid.NewGuid(),
                ServiceUri = serviceUri,
                ServiceName = serviceName,
                IsEnable = true
            };

            var clusters = _redisClient.SMembers<RpcServiceEntry>(serviceName);
            if (clusters == null)
                clusters = new RpcServiceEntry[] { serviceEntry };
            else
                clusters = clusters.Concat(new List<RpcServiceEntry> { serviceEntry }).ToArray();

            _redisClient.SPop(serviceName);
            _redisClient.SAdd(serviceName, clusters);
            return Task.CompletedTask;
        }

        public Task RemoveService<TService>()
        {
            var serviceName = typeof(TService).GetServiceName();
            return RemoveService(serviceName);
        }

        public Task RemoveService(string serviceName)
        {
            _redisClient.SPop(serviceName);
            return Task.CompletedTask;
        }
    }
}
