using System;
using System.Collections.Generic;
using System.Text;

namespace FakeRpc.Core.LoadBalance
{
    public interface ILoadBalanceStrategy
    {
        Uri Select(IEnumerable<Uri> uris);
    }
}
