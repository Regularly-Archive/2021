using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caching.AOP.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheableAttribute : Attribute
    {
        public string CacheKeyPrefix { get; set; }
        public int Expiration { get; set; }
    }
}
