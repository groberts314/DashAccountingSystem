using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using DashAccountingSystem.Data.Repositories;
using DashAccountingSystem.Models;

namespace DashAccountingSystem.Filters
{
    public class TenantViewDataFilterAttribute : TypeFilterAttribute
    {
        public TenantViewDataFilterAttribute()
            : base(typeof(TenantViewDataFilterAttributeImpl))
        {
        }

        private class TenantViewDataFilterAttributeImpl : IAsyncActionFilter
        {
            private readonly IAccountingPeriodRepository _accountingPeriodRepository = null;
            private readonly ITenantRepository _tenantRepository = null;
            private readonly ILogger<TenantViewDataFilterAttribute> _logger = null;

            public TenantViewDataFilterAttributeImpl(
                IAccountingPeriodRepository accountingPeriodRepository,
                ITenantRepository tenantRepository,
                ILogger<TenantViewDataFilterAttribute> logger)
            {
                _accountingPeriodRepository = accountingPeriodRepository;
                _tenantRepository = tenantRepository;
                _logger = logger;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                object tenantIdParameter = null;
                if (!context.RouteData.Values.TryGetValue("tenantId", out tenantIdParameter))
                {
                    throw new ArgumentNullException("tenantId", "Could not find tenant ID route parameter");
                }

                int tenantId = 0;
                if (!int.TryParse(tenantIdParameter.ToString(), out tenantId))
                {
                    _logger.LogError("Tenant ID parameter value {0} is not valid", tenantIdParameter);

                    context.Result = new BadRequestObjectResult(
                        new
                        {
                            Message = $"Tenant ID parameter value '{tenantIdParameter}' is not valid"
                        });

                    return;
                }

                // TODO: Verify context user has authorization for the Tenant...?
                //       Or is that the job of an authorization filter that fires earlier in the pipeline...?
                
                var tenant = await _tenantRepository.GetTenantAsync(tenantId);
                if (tenant == null)
                {
                    _logger.LogError("Tenant ID {0} could not be found", tenantId);
                    context.Result = new NotFoundResult();
                    return;
                }

                var accountingPeriods = await _accountingPeriodRepository.GetByTenantAsync(tenantId);
                var today = DateTime.Today; // TODO: User's time zone!

                var currentPeriod = accountingPeriods.FirstOrDefault(ap => ap.ContainsDate(today));
                if (currentPeriod == null)
                    currentPeriod = await _accountingPeriodRepository.FetchOrCreateAccountingPeriodAsync(
                        tenantId,
                        tenant.AccountingPeriodType,
                        today);

                object accountingPeriodIdParameter = null;
                int parsedAccountingPeriodId;
                int? accountingPeriodId = null;

                if (context.RouteData.Values.TryGetValue("accountingPeriodId", out accountingPeriodIdParameter))
                {
                    if (!int.TryParse(accountingPeriodIdParameter.ToString(), out parsedAccountingPeriodId))
                    {
                        _logger.LogError(
                            "Accounting Period ID parameter value {0} is not valid",
                            accountingPeriodIdParameter);

                        context.Result = new BadRequestObjectResult(
                            new
                            {
                                Message = $"Accounting Period ID parameter value '{accountingPeriodIdParameter}' is not valid"
                            });

                        return;
                    }

                    accountingPeriodId = parsedAccountingPeriodId;
                }

                var selectedPeriod = accountingPeriodId.HasValue
                    ? accountingPeriods.FirstOrDefault(ap => ap.Id == accountingPeriodId)
                    : currentPeriod;

                if (context.Controller is Controller controller)
                {
                    controller.ViewBag.Tenant = tenant;
                    controller.ViewBag.AccountingPeriods = accountingPeriods
                        .Select(ap =>
                        {
                            var viewModel = AccountingPeriodViewModel.FromModel(ap);
                            viewModel.Selected = viewModel.Id == selectedPeriod.Id;
                            viewModel.Current = viewModel.Id == currentPeriod.Id;
                            return viewModel;
                        })
                        .ToList();

                    var selectedPeriodViewModel = AccountingPeriodViewModel.FromModel(selectedPeriod);
                    selectedPeriodViewModel.Selected = true;
                    selectedPeriodViewModel.Current = selectedPeriodViewModel.Id == currentPeriod.Id;
                    controller.ViewBag.SelectedPeriod = selectedPeriodViewModel;
                }
                else
                {
                    _logger.LogWarning("Controller was of unexpected type; unable to access ViewBag data");
                }

                await next();
            }
        }
    }
}
