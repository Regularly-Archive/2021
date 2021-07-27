using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FakeRpc.Core.Discovery
{
    public interface IRpcServiceDiscovery
    {
        Task RegisterService<TService>(Uri serviceUri);
        Task RegisterService(string serviceName, Uri uri, string serviceNamespace);

        Task RemoveService<TService>(string serviceNamespace);
        Task RemoveService(string serviceName, string serviceNamespace);

        Uri GetService<TService>(string serviceNamespace);
        Uri GetService(string serviceName, string serviceNamespace);
    }
}
