using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using CSRedis;

namespace FakeRpc.Core.Discovery
{
    public class RpcServiceDiscovery : IRpcServiceDiscovery
    {
        private CSRedisClient _redisClient;

        public RpcServiceDiscovery(CSRedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        public Uri GetService<TService>(string serviceNamespace)
        {
            var serviceName = typeof(TService).GetServiceName();
            return GetService(serviceName, serviceNamespace);
        }

        public Uri GetService(string serviceName, string serviceNamespace)
        {
            var clusters = _redisClient.SMembers<RpcServiceEntry>(serviceName);
            if (clusters != null && clusters.Any())
            {
                var cluster = clusters.FirstOrDefault(x => x.ServiceNamespace == serviceNamespace);
                if (cluster == null)
                    throw new ArgumentException($"Service {serviceNamespace}:{serviceName} can't be resolved.");

                return cluster.ServiceUri;
            }

            throw new ArgumentException($"Service {serviceNamespace}:{serviceName} can't be resolved.");
        }

        public Task RegisterService<TService>(Uri serviceUri)
        {
            var serviceName = typeof(TService).GetServiceName();
            var serviceNamespace = typeof(TService).Namespace;
            return RegisterService(serviceName, serviceUri, serviceNamespace);
        }

        public Task RegisterService(string serviceName, Uri serviceUri, string serviceNamespace)
        {
            var serviceEntry = new RpcServiceEntry()
            {
                ServiceId = Guid.NewGuid(),
                ServiceUri = serviceUri,
                ServiceName = serviceName,
                ServiceNamespace = serviceNamespace,
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

        public Task RemoveService<TService>(string serviceNamespace)
        {
            return Task.CompletedTask;
        }

        public Task RemoveService(string serviceName, string serviceNamespace)
        {
            return Task.CompletedTask;
        }
    }
}
