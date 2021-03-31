using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Sharding
{
    class ShardingByYearPolicy : IShardingPolicyProvider
    {
        public string GetShardingSuffix()
        {
            return $"{DateTime.Now.ToString("yyyy")}";
        }
    }
}
