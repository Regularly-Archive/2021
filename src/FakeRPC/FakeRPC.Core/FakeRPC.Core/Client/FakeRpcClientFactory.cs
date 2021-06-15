using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace FakeRpc.Core.Mvc
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

        public TClient Create<TClient>()
        {
            var httpClientFactory = _serviceProvider.GetService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(TClient));

            var clientProxy = DispatchProxy.Create<TClient, ClientProxyBase>();
            (clientProxy as ClientProxyBase).HttpClient = httpClient;
            (clientProxy as ClientProxyBase).ServiceName = GetServiceName<TClient>();
            return clientProxy;
        }

        public TClient Create<TClient>(Uri baseUri)
        {
            var proxy = DispatchProxy.Create<TClient, ClientProxyBase>();
            (proxy as ClientProxyBase).HttpClient = new HttpClient() { BaseAddress = baseUri };
            (proxy as ClientProxyBase).ServiceName = GetServiceName<TClient>();
            return proxy;
        }

        public TClient Create<TClient>(string baseUrl)
        {
            var baseUri = new Uri(baseUrl);
            return Create<TClient>(baseUri);
        }

        private string GetServiceName<TClient>()
        {
            if (!typeof(TClient).IsInterface)
                return typeof(TClient).Name;

            return typeof(TClient).Name.AsSpan().Slice(1).ToString();
        }
    }
}
