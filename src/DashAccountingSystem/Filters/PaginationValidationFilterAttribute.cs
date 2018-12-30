using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using DashAccountingSystem.Extensions;
using DashAccountingSystem.Models;

namespace DashAccountingSystem.Filters
{
    public class PaginationValidationFilterAttribute : ActionFilterAttribute
    {
        public const string PaginationParameterKey = "pagination";
        public const string PageSizeQueryStringKey = "pageSize";
        public const string PageNumberQueryStringKey = "pageNumber";

        public int DefaultPageSize { get; private set; } = 10; // TODO: Make Configurable
        public int MaximumPageSize { get; private set; } = 100; // TODO: Make Configurable

        public PaginationValidationFilterAttribute(
            /*IOptionsSnapshot<PaginationSettings> paginationSettingsOptions,*/ // TODO: If we want pagination default settings configurable
            int defaultPageSize = 0,
            int maximumPageSize = 0) : base()
        {
            if (defaultPageSize > 0)
                DefaultPageSize = defaultPageSize;

            if (maximumPageSize > 0)
                MaximumPageSize = maximumPageSize;

            DefaultPageSize = Math.Min(DefaultPageSize, MaximumPageSize); // validate default vs. max
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            PaginationViewModel pagination = null;

            if (context.ActionArguments.ContainsKey(PaginationParameterKey))
            {
                // Coalesce pagination request with valid values
                pagination = context.ActionArguments[PaginationParameterKey] as PaginationViewModel;
                if (pagination == null)
                {
                    pagination = new PaginationViewModel(); // Should never happen, but in case a dev names a parameter "pagination" that's not a Pagination type
                }

                CoalesceValues(pagination);
            }

            if (context.Controller is Controller controller)
            {
                controller.ViewBag.Pagination = pagination?.ToModel() ?? new Data.Models.Pagination();
            }

            await next();
        }

        private void CoalesceValues(PaginationViewModel pagination)
        {
            // Coalesce with specified defaults
            pagination.PageNumber = pagination?.PageNumber ?? 1; // A value other than 1 for page number doesn't make sense as a default
            pagination.PageSize = pagination?.PageSize ?? DefaultPageSize;

            // Reconcile with acceptable ranges (user could pass in unacceptable values -- these should not throw exceptions, but be set to acceptable values)
            pagination.PageNumber = pagination.PageNumber.EnsureIsPositive(1); // page number is only meaningful for > 0
            pagination.PageSize = pagination.PageSize.CoalesceInRange(1, MaximumPageSize); // Page size must be between 1 and max page size

            // TODO: Sorting if needed
        }
    }
}
