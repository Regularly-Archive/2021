using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Sharding.MulitiTenancy
{
    public class TenantInfoProvider : ITenantInfoProvider
    {
        private const string X_TENANT_ID = "X-TenantId";

        private readonly IHttpContextAccessor _httpContextAccessor;
        public TenantInfoProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetTenantId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && httpContext.Request.Headers.ContainsKey(X_TENANT_ID))
                return httpContext.Request.Headers[X_TENANT_ID].FirstOrDefault();

            return null;
        }
    }
}
