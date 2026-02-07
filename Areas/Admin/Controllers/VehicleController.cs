using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class VehicleController : Controller
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<VehicleController> _logger;

        public VehicleController(IVehicleService vehicleService, ILogger<VehicleController> logger)
        {
            _vehicleService = vehicleService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? searchTerm, VehicleStatus? status, FuelType? fuelType, int page = 1)
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

            if (status.HasValue)
            {
                vehicles = vehicles.Where(v => v.Status == status.Value);
            }

            if (fuelType.HasValue)
            {
                vehicles = vehicles.Where(v => v.FuelType == fuelType.Value);
            }

            var pageSize = 10;
            var paginatedList = PaginatedList<Vehicle>.Create(vehicles, page, pageSize);

            ViewBag.Statuses = new SelectList(Enum.GetValues<VehicleStatus>());
            ViewBag.FuelTypes = new SelectList(Enum.GetValues<FuelType>());
            ViewData["SelectedStatus"] = status;
            ViewData["SelectedFuelType"] = fuelType;
            ViewBag.PageIndex = paginatedList.PageIndex;
            ViewBag.TotalPages = paginatedList.TotalPages;
            ViewBag.TotalCount = paginatedList.TotalCount;
            ViewBag.BaseUrl = Url.Action("Index");

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(searchTerm)) queryParams.Add($"&searchTerm={searchTerm}");
            if (status.HasValue) queryParams.Add($"&status={status.Value}");
            if (fuelType.HasValue) queryParams.Add($"&fuelType={fuelType.Value}");
            ViewBag.QueryString = string.Join("", queryParams);

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
                try
                {
                    await _vehicleService.CreateVehicleAsync(vehicle);
                    TempData["SuccessMessage"] = "Vehicle created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error creating vehicle");
                    if (ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true ||
                        ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        ModelState.AddModelError("RegistrationNumber", "A vehicle with this registration number already exists.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "A database error occurred while creating the vehicle.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating vehicle");
                    TempData["ErrorMessage"] = "An unexpected error occurred while creating the vehicle.";
                }
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
                try
                {
                    await _vehicleService.UpdateVehicleAsync(vehicle);
                    TempData["SuccessMessage"] = "Vehicle updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error updating vehicle {Id}", id);
                    if (ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true ||
                        ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        ModelState.AddModelError("RegistrationNumber", "A vehicle with this registration number already exists.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "A database error occurred while updating the vehicle.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating vehicle {Id}", id);
                    TempData["ErrorMessage"] = "An unexpected error occurred while updating the vehicle.";
                }
            }
            return View(vehicle);
        }

        public async Task<IActionResult> Details(int id)
        {
            var vehicle = await _vehicleService.GetVehicleWithMaintenanceAsync(id);
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

            var (tripCount, maintenanceCount) = await _vehicleService.GetDependencyCountsAsync(id);
            ViewBag.TripCount = tripCount;
            ViewBag.MaintenanceCount = maintenanceCount;

            return View(vehicle);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var canDelete = await _vehicleService.CanDeleteAsync(id);
                if (!canDelete)
                {
                    TempData["ErrorMessage"] = "Cannot delete this vehicle because it has associated trips. Please remove or reassign the trips first.";
                    return RedirectToAction(nameof(Index));
                }

                await _vehicleService.DeleteVehicleAsync(id);
                TempData["SuccessMessage"] = "Vehicle deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting vehicle {Id}", id);
                TempData["ErrorMessage"] = "Cannot delete this vehicle because it has related records.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vehicle {Id}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the vehicle.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Export()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            var columns = new Dictionary<string, Func<Vehicle, object>>
            {
                { "Registration Number", v => v.RegistrationNumber },
                { "Make", v => v.Make },
                { "Model", v => v.Model },
                { "Year", v => v.Year },
                { "Fuel Type", v => v.FuelType },
                { "Status", v => v.Status },
                { "Mileage", v => v.Mileage }
            };

            var csvData = Helpers.CsvExportHelper.ExportToCsv(vehicles, columns);
            return File(csvData, "text/csv", $"Vehicles_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}
