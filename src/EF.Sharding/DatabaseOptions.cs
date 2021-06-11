using System;
using System.Collections.Generic;
using System.Text;

namespace EF.Sharding
{
    public class DatabaseOptions
    {
        public string Default { get; set; }
        public List<TenantInfo> MultiTenants { get; set; }
    }

    public class TenantInfo
    {
        public string TenantId { get; set; }
        public string ConnectionString { get; set; }
    }
}
