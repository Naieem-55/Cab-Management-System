using Cab_Management_System.Models;
using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cab_Management_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class VehicleController : Controller
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public async Task<IActionResult> Index(string? searchTerm, int page = 1)
        {
            IEnumerable<Vehicle> vehicles;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                vehicles = await _vehicleService.SearchVehiclesAsync(searchTerm.Trim());
                ViewBag.SearchTerm = searchTerm;
            }
            else
            {
                vehicles = await _vehicleService.GetAllVehiclesAsync();
            }

            var pageSize = 10;
            var paginatedList = PaginatedList<Vehicle>.Create(vehicles, page, pageSize);

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
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                await _vehicleService.CreateVehicleAsync(vehicle);
                TempData["StatusMessage"] = "Vehicle created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
                return NotFound();

            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vehicle vehicle)
        {
            if (id != vehicle.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _vehicleService.UpdateVehicleAsync(vehicle);
                TempData["StatusMessage"] = "Vehicle updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        public async Task<IActionResult> Details(int id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
                return NotFound();

            return View(vehicle);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
                return NotFound();

            return View(vehicle);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _vehicleService.DeleteVehicleAsync(id);
            TempData["StatusMessage"] = "Vehicle deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
