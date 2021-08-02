using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FakeRpc.Core.LoadBalance
{
    public class RandomLoadBalanceStrategy : ILoadBalanceStrategy
    {
        public Uri Select(IEnumerable<Uri> uris)
        {
            var rand = new Random();
            var index = rand.Next(0, uris.Count());
            return uris.ElementAt(index);
        }
    }
}
