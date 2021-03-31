using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Sharding.MulitiTenancy
{
    class MockTenantInfoProvider : ITenantInfoProvider
    {
        public string GetTenantId()
        {
            return "01";
        }
    }
}
