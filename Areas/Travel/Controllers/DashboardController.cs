using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cab_Management_System.Areas.Travel.Controllers
{
    [Area("Travel")]
    [Authorize(Roles = "TravelManager")]
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
