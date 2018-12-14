using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class StatementsIndexViewModel : BaseTenantViewModel
    {
        public StatementsIndexViewModel(Tenant tenant, string sectionTitle = "Statements")
            : base(tenant, sectionTitle)
        {

        }
    }
}
