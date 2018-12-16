using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using DashAccountingSystem.Models;

namespace DashAccountingSystem.Filters
{
    public class JournalEntryViewModelValidationFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogger _logger;

        public JournalEntryViewModelValidationFilterAttribute(
            ILogger<JournalEntryViewModelValidationFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ModelState.IsValid)
            {
                // TODO: Extract the view model and perform additional validation:
                // * Must contain at least two accounts of each asset type with non-zero values
                // * Must balance (debits must equal credits)
            }

            return base.OnActionExecutionAsync(context, next);
        }
    }
}
