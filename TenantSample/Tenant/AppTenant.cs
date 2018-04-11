using System.Collections.Generic;

namespace TenantSample.Tenant
{
	public class AppTenant
    {
        public static AppTenant Empty = new AppTenant { Name = "Empty" };
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Hostnames { get; set; }
    }
}
