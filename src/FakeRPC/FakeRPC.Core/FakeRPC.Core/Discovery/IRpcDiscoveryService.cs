using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FakeRpc.Core.Discovery
{
    public interface IRpcDiscoveryService
    {
        Task RegisterService<TService>(Uri uri);
        Task RegisterService(string serviceName, Uri uri);

        Task RemoveService<TService>();
        Task RemoveService(string serviceName);

        Task DisableService<TService>();
        Task DisableService(string serviceName);

        Uri GetService<TService>();
        Uri GetService(string serviceName);
    }
}
