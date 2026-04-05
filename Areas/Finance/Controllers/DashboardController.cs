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

        public async Task<IActionResult> Index()
        {
            var model = await _dashboardService.GetFinanceDashboardAsync();
            return View(model);
        }
    }
}
