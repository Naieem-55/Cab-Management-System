using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;
using CabManagementSystem.Models.ViewModels;
using CabManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace CabManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRouteService _routeService;
        private readonly IPricingService _pricingService;

        public HomeController(IRouteService routeService, IPricingService pricingService)
        {
            _routeService = routeService;
            _pricingService = pricingService;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole(nameof(UserRole.Admin)))
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                if (User.IsInRole(nameof(UserRole.FinanceManager)))
                    return RedirectToAction("Index", "Dashboard", new { area = "Finance" });
                if (User.IsInRole(nameof(UserRole.HRManager)))
                    return RedirectToAction("Index", "Dashboard", new { area = "HR" });
                if (User.IsInRole(nameof(UserRole.TravelManager)))
                    return RedirectToAction("Index", "Dashboard", new { area = "Travel" });
                if (User.IsInRole(nameof(UserRole.Customer)))
                    return RedirectToAction("Index", "Dashboard", new { area = "CustomerPortal" });
            }

            var routes = (await _routeService.GetAllRoutesAsync()).ToList();
            var model = new FareEstimatorViewModel
            {
                AvailableRoutes = new SelectList(
                    routes.Select(r => new { r.Id, Display = $"{r.Origin} → {r.Destination} ({r.Distance} km)" }),
                    "Id", "Display")
            };

            return View(model);
        }

        /// <summary>Public fare quote for the landing-page estimator. Read-only, no login required.</summary>
        [HttpGet]
        public async Task<IActionResult> FareEstimate(int routeId, DateTime tripDate)
        {
            var route = await _routeService.GetRouteByIdAsync(routeId);
            if (route == null)
                return NotFound();

            var quote = await _pricingService.GetQuoteAsync(route.BaseCost, tripDate);
            return Json(new
            {
                route = $"{route.Origin} → {route.Destination}",
                distance = route.Distance,
                baseFare = quote.BaseFare,
                surcharge = quote.Surcharge,
                surchargeLabel = quote.SurchargeLabel,
                total = quote.Total
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
