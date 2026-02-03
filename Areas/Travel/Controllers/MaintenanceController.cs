using Cab_Management_System.Models;
using Cab_Management_System.Models.ViewModels;
using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cab_Management_System.Areas.Travel.Controllers
{
    [Area("Travel")]
    [Authorize(Roles = "TravelManager")]
    public class MaintenanceController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly IVehicleService _vehicleService;

        public MaintenanceController(IMaintenanceService maintenanceService, IVehicleService vehicleService)
        {
            _maintenanceService = maintenanceService;
            _vehicleService = vehicleService;
        }

        public async Task<IActionResult> Index(string? searchTerm)
        {
            var records = await _maintenanceService.GetAllMaintenanceRecordsAsync();
            return View(records);
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
            await _maintenanceService.DeleteMaintenanceAsync(id);
            TempData["SuccessMessage"] = "Maintenance record deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateVehiclesAsync(MaintenanceViewModel model)
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            model.AvailableVehicles = new SelectList(vehicles, "Id", "RegistrationNumber");
        }
    }
}
