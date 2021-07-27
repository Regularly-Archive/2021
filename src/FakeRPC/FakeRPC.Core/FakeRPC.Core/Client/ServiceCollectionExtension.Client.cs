using CSRedis;
using FakeRpc.Core.Discovery;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace FakeRpc.Core.Client
{
    public static class ServiceCollectionExtension
    {
        public static void AddFakeRpcClient<TClient>(this IServiceCollection services, Action<HttpClient> configureClient)
        {
            services.AddHttpClient(typeof(TClient).Name.AsSpan().Slice(1).ToString(), configureClient);
            services.AddSingleton<FakeRpcClientFactory>();
        }

        public static void AddFakeRpcClient<TClient>(this IServiceCollection services, ServiceDiscoveryOptions serviceDiscoveryOptions, Action<HttpClient> configureClient = null)
        {
            services.AddSingleton<FakeRpcClientFactory>();
            services.AddSingleton<IRpcServiceDiscovery, RpcServiceDiscovery>();
            services.AddSingleton<CSRedisClient>(sp =>
            {
                var client = new CSRedisClient(serviceDiscoveryOptions.DiscoveryServer);
                RedisHelper.Initialization(client);
                return client;
            });

            var serviceProvider = services.BuildServiceProvider();
            var serviceDiscovery = serviceProvider.GetService<IRpcServiceDiscovery>();
            var serviceUri = serviceDiscovery.GetService<TClient>(serviceDiscoveryOptions.ServiceNamespace);

            services.AddFakeRpcClient<TClient>(client =>
            {
                client.BaseAddress = serviceUri;
                client.DefaultRequestVersion = new Version(2, 0);
                configureClient?.Invoke(client);
            });
        }

        public static void AddFakeRpcCallsFactory(this IServiceCollection services, Func<HttpClient, IFakeRpcCalls> factory = null)
        {
            if (factory == null)
                factory = httpClient => new DefaultFakeRpcCalls(httpClient);

            services.AddSingleton(factory);
        }
    }
}
