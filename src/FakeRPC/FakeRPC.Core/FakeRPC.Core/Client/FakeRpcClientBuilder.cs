using Consul;
using FakeRpc.Core.Client;
using FakeRpc.Core.Discovery;
using FakeRpc.Core.Discovery.Consul;
using FakeRpc.Core.Discovery.Redis;
using FakeRpc.Core.LoadBalance;
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

        public FakeRpcClientBuilder AddRpcCallsFactory(Func<HttpClient, IFakeRpcCalls> factory = null)
        {
            if (factory == null)
                factory = httpClient => new DefaultFakeRpcCalls(httpClient);

            _services.AddSingleton(factory);
            return this;
        }

        public FakeRpcClientBuilder EnableServiceDiscovery<TServiceDiscovery>(Func<IServiceProvider, TServiceDiscovery> serviceDiscoveryFactory = null) where TServiceDiscovery : class, IServiceDiscovery
        {
            if (serviceDiscoveryFactory != null)
                _services.AddSingleton<TServiceDiscovery>(serviceDiscoveryFactory);
            else
                _services.AddSingleton<IServiceDiscovery, TServiceDiscovery>();

            return this;
        }

        public FakeRpcClientBuilder EnableConsulServiceDiscovery(Action<ConsulServiceDiscoveryOptions> setupAction)
        {
            var options = new ConsulServiceDiscoveryOptions();
            setupAction?.Invoke(options);

            _services.AddSingleton(options);
            _services.AddSingleton<IServiceDiscovery, ConsulServiceDiscovery>();
            return this;
        }

        public FakeRpcClientBuilder EnableRedisServiceDiscovery(Action<RedisServiceDiscoveryOptions> setupAction)
        {
            var options = new RedisServiceDiscoveryOptions();
            setupAction?.Invoke(options);

            _services.AddSingleton(options);
            _services.AddSingleton<IServiceDiscovery, RedisServiceDiscovery>();
            return this;
        }

        public FakeRpcClientBuilder EnableLoadBalance<TStrategy>() where TStrategy: class, ILoadBalanceStrategy
        {
            _services.AddTransient<ILoadBalanceStrategy, TStrategy>();
            return this;
        }


        public void Build()
        {
            var serviceProvider = _services.BuildServiceProvider(); 
        }
    }
}
