using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FakeRpc.Core.Discovery
{
    public interface IServiceDiscovery
    {
        Uri GetService<TService>(string serviceNGroup = null);
        Uri GetService(string serviceName, string serviceGroup);
    }
}
