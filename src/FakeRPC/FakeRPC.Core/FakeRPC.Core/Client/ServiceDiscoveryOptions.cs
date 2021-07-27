using System;
using System.Collections.Generic;
using System.Text;

namespace FakeRpc.Core.Client
{
    public class ServiceDiscoveryOptions
    {
        public string DiscoveryServer { get; set; }
        public string ServiceNamespace { get; set; }

    }
}
