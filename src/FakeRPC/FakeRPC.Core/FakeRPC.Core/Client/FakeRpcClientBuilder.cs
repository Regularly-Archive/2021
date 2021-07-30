using Consul;
using FakeRpc.Core.Client;
using FakeRpc.Core.Discovery;
using FakeRpc.Core.Discovery.Consul;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace FakeRpc.Core.Client
{
    public class FakeRpcClientBuilder
    {
        private readonly IServiceCollection _services;

        public FakeRpcClientBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public FakeRpcClientBuilder AddRpcClient<TClient>(Action<HttpClient> configureClient)
        {
            _services.AddHttpClient(typeof(TClient).GetServiceName(), configureClient);
            _services.AddSingleton<FakeRpcClientFactory>();
            return this;
        }

        //public static void AddRpcClient<TClient>(this IServiceCollection services, ServiceDiscoveryOptions serviceDiscoveryOptions, Action<HttpClient> configureClient = null)
        //{
        //    services.AddSingleton<FakeRpcClientFactory>();
        //    services.AddSingleton<IServiceDiscovery, RedisServiceDiscovery>();
        //    services.AddSingleton<CSRedisClient>(sp =>
        //    {
        //        var client = new CSRedisClient(serviceDiscoveryOptions.DiscoveryServer);
        //        RedisHelper.Initialization(client);
        //        return client;
        //    });

        //    var serviceProvider = services.BuildServiceProvider();
        //    var serviceDiscovery = serviceProvider.GetService<IServiceDiscovery>();
        //    var serviceUri = serviceDiscovery.GetService<TClient>(serviceDiscoveryOptions.ServiceNamespace);

        //    services.AddFakeRpcClient<TClient>(client =>
        //    {
        //        client.BaseAddress = serviceUri;
        //        client.DefaultRequestVersion = new Version(2, 0);
        //        configureClient?.Invoke(client);
        //    });
        //}

        public FakeRpcClientBuilder AddRpcCallsFactory(Func<HttpClient, IFakeRpcCalls> factory = null)
        {
            if (factory == null)
                factory = httpClient => new DefaultFakeRpcCalls(httpClient);

            _services.AddSingleton(factory);
            return this;
        }

        public FakeRpcClientBuilder EnableConsulServiceDiscovery<TServiceDiscovery>(Func<IServiceProvider, TServiceDiscovery> serviceDiscoveryFactory = null) where TServiceDiscovery : class, IServiceDiscovery
        {
            if (serviceDiscoveryFactory != null)
                _services.AddSingleton<TServiceDiscovery>(serviceDiscoveryFactory);
            else
                _services.AddSingleton<IServiceDiscovery, TServiceDiscovery>();

            return this;
        }

        public FakeRpcClientBuilder EnableConsulServiceDiscovery(ConsulServiceDiscoveryOptions options)
        {
            _services.AddSingleton<IConsulClient, ConsulClient>(_ => new ConsulClient(consulConfig =>
            {
                consulConfig.Address = new Uri(options.BaseUrl);
            }));

            _services.AddSingleton<IServiceDiscovery, ConsulServiceDiscovery>();
            return this;
        }


        public void Build()
        {
            var serviceProvider = _services.BuildServiceProvider(); 
        }
    }
}
