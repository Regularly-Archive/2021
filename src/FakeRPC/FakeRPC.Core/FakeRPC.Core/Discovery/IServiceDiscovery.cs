using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FakeRpc.Core.Discovery
{
    public interface IServiceDiscovery
    {
        IEnumerable<Uri> GetService<TService>(string serviceNGroup = null);
        IEnumerable<Uri> GetService(string serviceName, string serviceGroup);
    }
}
