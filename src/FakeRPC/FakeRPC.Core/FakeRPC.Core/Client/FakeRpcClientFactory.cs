using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace FakeRpc.Core.Client
{
    public class FakeRpcClientFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public FakeRpcClientFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public FakeRpcClientFactory()
        {
            _serviceProvider = new ServiceCollection().BuildServiceProvider();
        }

        public TClient Create<TClient>(Func<HttpClient, IFakeRpcCalls> rpcCallsFactory = null)
        {
            var httpClientFactory = _serviceProvider.GetService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(GetServiceName<TClient>());

            var clientProxy = DispatchProxy.Create<TClient, ClientProxyBase>();
            (clientProxy as ClientProxyBase).HttpClient = httpClient;
            (clientProxy as ClientProxyBase).ServiceName = GetServiceName<TClient>();
            (clientProxy as ClientProxyBase).RpcCalls = rpcCallsFactory == null ? 
                new DefaultFakeRpcCalls(httpClient) : rpcCallsFactory(httpClient);

            return clientProxy;
        }

        public TClient Create<TClient>(Uri baseUri, Func<HttpClient, IFakeRpcCalls> rpcCallsFactory = null)
        {
            var clientProxy = DispatchProxy.Create<TClient, ClientProxyBase>();
            var httpClient = new HttpClient() { BaseAddress = baseUri };

            (clientProxy as ClientProxyBase).HttpClient = new HttpClient() { BaseAddress = baseUri };
            (clientProxy as ClientProxyBase).ServiceName = GetServiceName<TClient>();
            (clientProxy as ClientProxyBase).RpcCalls = rpcCallsFactory == null ?
                new DefaultFakeRpcCalls(httpClient) : rpcCallsFactory(httpClient);

            return clientProxy;
        }

        public TClient Create<TClient>(string baseUrl, Func<HttpClient, IFakeRpcCalls> rpcCallsFactory = null)
        {
            var baseUri = new Uri(baseUrl);
            return Create<TClient>(baseUri, rpcCallsFactory);
        }

        private string GetServiceName<TClient>()
        {
            if (!typeof(TClient).IsInterface)
                return typeof(TClient).Name;

            return typeof(TClient).Name.AsSpan().Slice(1).ToString();
        }
    }
}
