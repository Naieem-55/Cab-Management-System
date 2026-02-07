using Cab_Management_System.Models;
using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RouteController : Controller
    {
        private readonly IRouteService _routeService;
        private readonly ILogger<RouteController> _logger;

        public RouteController(IRouteService routeService, ILogger<RouteController> logger)
        {
            _routeService = routeService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? searchTerm, int page = 1)
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

            var pageSize = 10;
            var paginatedList = PaginatedList<Models.Route>.Create(routes, page, pageSize);

            ViewBag.PageIndex = paginatedList.PageIndex;
            ViewBag.TotalPages = paginatedList.TotalPages;
            ViewBag.TotalCount = paginatedList.TotalCount;
            ViewBag.BaseUrl = Url.Action("Index");
            ViewBag.QueryString = !string.IsNullOrEmpty(searchTerm) ? $"&searchTerm={searchTerm}" : "";

            return View(paginatedList);
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
                try
                {
                    await _routeService.CreateRouteAsync(route);
                    TempData["SuccessMessage"] = "Route created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error creating route");
                    TempData["ErrorMessage"] = "A database error occurred while creating the route.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating route");
                    TempData["ErrorMessage"] = "An unexpected error occurred while creating the route.";
                }
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
                try
                {
                    await _routeService.UpdateRouteAsync(route);
                    TempData["SuccessMessage"] = "Route updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error updating route {Id}", id);
                    TempData["ErrorMessage"] = "A database error occurred while updating the route.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating route {Id}", id);
                    TempData["ErrorMessage"] = "An unexpected error occurred while updating the route.";
                }
            }
            return View(route);
        }

        public async Task<IActionResult> Details(int id)
        {
            var route = await _routeService.GetRouteWithTripsAsync(id);
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

            var tripCount = await _routeService.GetTripCountAsync(id);
            ViewBag.TripCount = tripCount;

            return View(route);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var canDelete = await _routeService.CanDeleteAsync(id);
                if (!canDelete)
                {
                    TempData["ErrorMessage"] = "Cannot delete this route because it has associated trips. Please remove or reassign the trips first.";
                    return RedirectToAction(nameof(Index));
                }

                await _routeService.DeleteRouteAsync(id);
                TempData["SuccessMessage"] = "Route deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting route {Id}", id);
                TempData["ErrorMessage"] = "Cannot delete this route because it has related records.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting route {Id}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the route.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
