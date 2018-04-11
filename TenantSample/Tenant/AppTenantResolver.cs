using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TenantSample.Tenant
{
    public class AppTenantResolver : MemoryCacheTenantResolver<AppTenant>
    {
        private readonly Dictionary<string, Tuple<int, string>> mappings = new Dictionary<string, Tuple<int, string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "localhost:3001", new Tuple<int, string>( 1, "Tenant 1") },
            { "localhost:3002", new Tuple<int, string>( 2, "Tenant 2") },
            { "localhost:3003", new Tuple<int, string>( 3, "Tenant 3") },
        };


        public AppTenantResolver(IMemoryCache cache, ILoggerFactory loggerFactory) 
            : base(cache, loggerFactory)
        {
            
        }

        protected override string GetContextIdentifier(HttpContext context)
        {
            return context.Request.Host.Value.ToLower();
        }

        protected override IEnumerable<string> GetTenantIdentifiers(TenantContext<AppTenant> context)
        {
            return context.Tenant.Hostnames;
        }

        protected override Task<TenantContext<AppTenant>> ResolveAsync(HttpContext context)
        {
            Tuple<int,string> tenant = null;
            TenantContext<AppTenant> tenantContext = null;

            if (mappings.TryGetValue(context.Request.Host.Value, out tenant))
            {
                tenantContext = new TenantContext<AppTenant>(
                    new AppTenant { Id = tenant.Item1, Name = tenant.Item2, Hostnames = new[] { context.Request.Host.Value } });
            }
            else
            {
                tenantContext = new TenantContext<AppTenant>(
                    AppTenant.Empty);
            }

            return Task.FromResult(tenantContext);
        }

        protected override MemoryCacheEntryOptions CreateCacheEntryOptions()
        {
            return base.CreateCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));
        }
    }
}
