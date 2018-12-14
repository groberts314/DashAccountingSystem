using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public abstract class BaseTenantViewModel
    {
        public Tenant Tenant { get; private set; }

        public string SectionTitle { get; private set; }

        public BaseTenantViewModel(Tenant tenant, string sectionTitle)
        {
            Tenant = tenant;
            SectionTitle = sectionTitle;
        }
    }
}
