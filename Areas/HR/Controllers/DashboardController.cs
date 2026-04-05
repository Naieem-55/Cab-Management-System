using CabManagementSystem.Models.Enums;
using CabManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CabManagementSystem.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize(Roles = nameof(UserRole.HRManager))]
    public class DashboardController : Controller
    {
        private readonly IHRDashboardService _dashboardService;

        public DashboardController(IHRDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _dashboardService.GetHRDashboardAsync();
            return View(model);
        }
    }
}
