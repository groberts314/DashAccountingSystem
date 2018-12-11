using System;
using Microsoft.AspNetCore.Identity;

namespace DashAccountingSystem.Data.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string Description { get; set; }
    }
}
