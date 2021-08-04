using System;
using System.Collections.Generic;
using System.Text;

namespace FakeRpc.Core.Discovery.Redis
{
    public class RedisServiceDiscoveryOptions
    {
        public string RedisUrl { get; set; }
        public string RegisterEventTopic { get; set; } = "evt_service_register";
        public string UnregisterEventTopic { get; set; } = "evt_service_unregister";
    }
}
