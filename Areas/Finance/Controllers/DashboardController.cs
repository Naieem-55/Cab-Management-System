using CabManagementSystem.Models.Enums;
using CabManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CabManagementSystem.Areas.Finance.Controllers
{
    [Area("Finance")]
    [Authorize(Roles = nameof(UserRole.FinanceManager))]
    public class DashboardController : Controller
    {
        private readonly IFinanceDashboardService _dashboardService;

        public DashboardController(IFinanceDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index(int months = 6)
        {
            months = Helpers.DashboardRange.Normalize(months);
            ViewBag.SelectedMonths = months;
            var model = await _dashboardService.GetFinanceDashboardAsync(months);
            return View(model);
        }
    }
}
