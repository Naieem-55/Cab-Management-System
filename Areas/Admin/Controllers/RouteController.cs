using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cab_Management_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RouteController : Controller
    {
        private readonly IRouteService _routeService;

        public RouteController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        public async Task<IActionResult> Index(string? searchTerm)
        {
            IEnumerable<Models.Route> routes;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                routes = await _routeService.SearchRoutesAsync(searchTerm.Trim());
                ViewBag.SearchTerm = searchTerm;
            }
            else
            {
                routes = await _routeService.GetAllRoutesAsync();
            }

            return View(routes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Route route)
        {
            if (ModelState.IsValid)
            {
                await _routeService.CreateRouteAsync(route);
                TempData["StatusMessage"] = "Route created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(route);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var route = await _routeService.GetRouteByIdAsync(id);
            if (route == null)
                return NotFound();

            return View(route);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Models.Route route)
        {
            if (id != route.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _routeService.UpdateRouteAsync(route);
                TempData["StatusMessage"] = "Route updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(route);
        }

        public async Task<IActionResult> Details(int id)
        {
            var route = await _routeService.GetRouteByIdAsync(id);
            if (route == null)
                return NotFound();

            return View(route);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var route = await _routeService.GetRouteByIdAsync(id);
            if (route == null)
                return NotFound();

            return View(route);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _routeService.DeleteRouteAsync(id);
            TempData["StatusMessage"] = "Route deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
