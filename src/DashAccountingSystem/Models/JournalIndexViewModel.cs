using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class JournalIndexViewModel : BaseTenantViewModel
    {
        public JournalIndexViewModel(Tenant tenant, string sectionTitle = "Journal")
            : base(tenant, sectionTitle)
        {

        }
    }
}
