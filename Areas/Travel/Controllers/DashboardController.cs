using CabManagementSystem.Models.Enums;
using CabManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CabManagementSystem.Areas.Travel.Controllers
{
    [Area("Travel")]
    [Authorize(Roles = nameof(UserRole.TravelManager))]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _dashboardService.GetTravelDashboardAsync();
            return View(model);
        }
    }
}
