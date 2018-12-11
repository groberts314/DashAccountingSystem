using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DashAccountingSystem.Data.Models;
using System.Security.Claims;

namespace DashAccountingSystem.Security.Authentication
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store,
                                     IOptions<IdentityOptions> optionsAccessor,
                                     IPasswordHasher<ApplicationUser> passwordHasher,
                                     IEnumerable<IUserValidator<ApplicationUser>> userValidators,
                                     IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
                                     ILookupNormalizer keyNormalizer,
                                     IdentityErrorDescriber errors,
                                     IServiceProvider services,
                                     ILogger<UserManager<ApplicationUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public string GetUserFirstName(ClaimsPrincipal user)
        {
            return user?.FindFirstValue(ClaimTypes.GivenName);
        }

        public string GetUserFullName(ClaimsPrincipal user)
        {
            if (user == null)
                return null;

            var firstName = user.FindFirstValue(ClaimTypes.GivenName);
            var lastName = user.FindFirstValue(ClaimTypes.Surname);
            return $"{firstName} {lastName}".Trim();
        }

        public string GetUserLastName(ClaimsPrincipal user)
        {
            return user?.FindFirstValue(ClaimTypes.Surname);
        }
    }
}
