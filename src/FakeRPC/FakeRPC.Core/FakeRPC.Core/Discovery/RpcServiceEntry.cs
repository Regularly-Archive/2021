using System;
using System.Collections.Generic;
using System.Text;

namespace FakeRpc.Core.Discovery
{
    public class RpcServiceEntry
    {
        public bool IsEnable { get; set; }
        public Uri ServiceUri { get; set; }
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }
    }
}
