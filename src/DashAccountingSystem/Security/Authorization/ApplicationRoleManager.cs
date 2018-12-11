using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Security.Authorization
{
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(
            IRoleStore<ApplicationRole> store,
            IEnumerable<IRoleValidator<ApplicationRole>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<RoleManager<ApplicationRole>> logger)
            : base (store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}
