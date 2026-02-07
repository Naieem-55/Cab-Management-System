using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Models.ViewModels;
using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Areas.Travel.Controllers
{
    [Area("Travel")]
    [Authorize(Roles = "TravelManager")]
    public class MaintenanceController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<MaintenanceController> _logger;

        public MaintenanceController(IMaintenanceService maintenanceService, IVehicleService vehicleService, ILogger<MaintenanceController> logger)
        {
            _maintenanceService = maintenanceService;
            _vehicleService = vehicleService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? searchTerm, MaintenanceStatus? status, int? vehicleId, int page = 1)
        {
            IEnumerable<MaintenanceRecord> records;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                records = await _maintenanceService.SearchMaintenanceAsync(searchTerm.Trim());
                ViewData["SearchTerm"] = searchTerm;
            }
            else
            {
                records = await _maintenanceService.GetAllMaintenanceRecordsAsync();
            }

            if (status.HasValue)
            {
                records = records.Where(m => m.Status == status.Value);
            }

            if (vehicleId.HasValue)
            {
                records = records.Where(m => m.VehicleId == vehicleId.Value);
            }

            var pageSize = 10;
            var paginatedList = PaginatedList<MaintenanceRecord>.Create(records, page, pageSize);

            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            ViewBag.Vehicles = new SelectList(vehicles, "Id", "RegistrationNumber");
            ViewBag.Statuses = new SelectList(Enum.GetValues<MaintenanceStatus>());
            ViewData["SelectedStatus"] = status;
            ViewData["SelectedVehicleId"] = vehicleId;
            ViewBag.PageIndex = paginatedList.PageIndex;
            ViewBag.TotalPages = paginatedList.TotalPages;
            ViewBag.TotalCount = paginatedList.TotalCount;
            ViewBag.BaseUrl = Url.Action("Index");

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(searchTerm)) queryParams.Add($"&searchTerm={searchTerm}");
            if (status.HasValue) queryParams.Add($"&status={status.Value}");
            if (vehicleId.HasValue) queryParams.Add($"&vehicleId={vehicleId.Value}");
            ViewBag.QueryString = string.Join("", queryParams);

            return View(paginatedList);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new MaintenanceViewModel
            {
                Date = DateTime.Today
            };

            await PopulateVehiclesAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaintenanceViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var record = new MaintenanceRecord
                    {
                        VehicleId = model.VehicleId,
                        Description = model.Description,
                        Cost = model.Cost,
                        Date = model.Date,
                        NextMaintenanceDate = model.NextMaintenanceDate,
                        Status = model.Status
                    };

                    await _maintenanceService.CreateMaintenanceAsync(record);
                    TempData["SuccessMessage"] = "Maintenance record created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error creating maintenance record");
                    TempData["ErrorMessage"] = "A database error occurred while creating the maintenance record.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating maintenance record");
                    TempData["ErrorMessage"] = "An unexpected error occurred while creating the maintenance record.";
                }
            }

            await PopulateVehiclesAsync(model);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var record = await _maintenanceService.GetMaintenanceWithVehicleAsync(id);
            if (record == null)
                return NotFound();

            var model = new MaintenanceViewModel
            {
                Id = record.Id,
                VehicleId = record.VehicleId,
                Description = record.Description,
                Cost = record.Cost,
                Date = record.Date,
                NextMaintenanceDate = record.NextMaintenanceDate,
                Status = record.Status
            };

            await PopulateVehiclesAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MaintenanceViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var record = new MaintenanceRecord
                    {
                        Id = model.Id,
                        VehicleId = model.VehicleId,
                        Description = model.Description,
                        Cost = model.Cost,
                        Date = model.Date,
                        NextMaintenanceDate = model.NextMaintenanceDate,
                        Status = model.Status
                    };

                    await _maintenanceService.UpdateMaintenanceAsync(record);
                    TempData["SuccessMessage"] = "Maintenance record updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error updating maintenance record {Id}", id);
                    TempData["ErrorMessage"] = "A database error occurred while updating the maintenance record.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating maintenance record {Id}", id);
                    TempData["ErrorMessage"] = "An unexpected error occurred while updating the maintenance record.";
                }
            }

            await PopulateVehiclesAsync(model);
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var record = await _maintenanceService.GetMaintenanceWithVehicleAsync(id);
            if (record == null)
                return NotFound();

            return View(record);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var record = await _maintenanceService.GetMaintenanceWithVehicleAsync(id);
            if (record == null)
                return NotFound();

            return View(record);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _maintenanceService.DeleteMaintenanceAsync(id);
                TempData["SuccessMessage"] = "Maintenance record deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting maintenance record {Id}", id);
                TempData["ErrorMessage"] = "Cannot delete this maintenance record because it has related records.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting maintenance record {Id}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the maintenance record.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Export()
        {
            var records = await _maintenanceService.GetAllMaintenanceRecordsAsync();
            var columns = new Dictionary<string, Func<MaintenanceRecord, object>>
            {
                { "Vehicle", m => m.Vehicle?.RegistrationNumber ?? "" },
                { "Description", m => m.Description },
                { "Cost", m => m.Cost },
                { "Date", m => m.Date.ToString("yyyy-MM-dd") },
                { "Next Maintenance", m => m.NextMaintenanceDate?.ToString("yyyy-MM-dd") ?? "" },
                { "Status", m => m.Status }
            };

            var csvData = Helpers.CsvExportHelper.ExportToCsv(records, columns);
            return File(csvData, "text/csv", $"Maintenance_{DateTime.Now:yyyyMMdd}.csv");
        }

        private async Task PopulateVehiclesAsync(MaintenanceViewModel model)
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            model.AvailableVehicles = new SelectList(vehicles, "Id", "RegistrationNumber");
        }
    }
}
