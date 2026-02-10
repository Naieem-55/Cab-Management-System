using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Models.ViewModels;
using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize(Roles = "HRManager")]
    public class DriverController : Controller
    {
        private readonly IDriverService _driverService;
        private readonly IEmployeeService _employeeService;
        private readonly IDriverRatingService _driverRatingService;
        private readonly ILogger<DriverController> _logger;

        public DriverController(IDriverService driverService, IEmployeeService employeeService, IDriverRatingService driverRatingService, ILogger<DriverController> logger)
        {
            _driverService = driverService;
            _employeeService = employeeService;
            _driverRatingService = driverRatingService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? searchTerm, int page = 1)
        {
            IEnumerable<Driver> drivers;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                drivers = await _driverService.SearchDriversAsync(searchTerm.Trim());
                ViewBag.SearchTerm = searchTerm;
            }
            else
            {
                drivers = await _driverService.GetAllDriversAsync();
            }

            var pageSize = 10;
            var paginatedList = PaginatedList<Driver>.Create(drivers, page, pageSize);

            ViewBag.PageIndex = paginatedList.PageIndex;
            ViewBag.TotalPages = paginatedList.TotalPages;
            ViewBag.TotalCount = paginatedList.TotalCount;
            ViewBag.BaseUrl = Url.Action("Index");
            ViewBag.QueryString = !string.IsNullOrEmpty(searchTerm) ? $"&searchTerm={searchTerm}" : "";

            return View(paginatedList);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var allEmployees = await _employeeService.GetEmployeesByPositionAsync(EmployeePosition.Driver);
            var allDrivers = await _driverService.GetAllDriversAsync();
            var driverEmployeeIds = allDrivers.Select(d => d.EmployeeId).ToHashSet();
            var availableEmployees = allEmployees.Where(e => !driverEmployeeIds.Contains(e.Id)).ToList();

            var model = new DriverViewModel
            {
                AvailableEmployees = new SelectList(availableEmployees, "Id", "Name")
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DriverViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var driver = new Driver
                    {
                        EmployeeId = model.EmployeeId,
                        LicenseNumber = model.LicenseNumber,
                        LicenseExpiry = model.LicenseExpiry,
                        Status = model.Status
                    };

                    await _driverService.CreateDriverAsync(driver);
                    TempData["SuccessMessage"] = "Driver created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error creating driver");
                    if (ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true ||
                        ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        ModelState.AddModelError("LicenseNumber", "A driver with this license number already exists.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "A database error occurred while creating the driver.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating driver");
                    TempData["ErrorMessage"] = "An unexpected error occurred while creating the driver.";
                }
            }

            var allEmployees = await _employeeService.GetEmployeesByPositionAsync(EmployeePosition.Driver);
            var allDrivers = await _driverService.GetAllDriversAsync();
            var driverEmployeeIds = allDrivers.Select(d => d.EmployeeId).ToHashSet();
            var availableEmployees = allEmployees.Where(e => !driverEmployeeIds.Contains(e.Id)).ToList();
            model.AvailableEmployees = new SelectList(availableEmployees, "Id", "Name");

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var driver = await _driverService.GetDriverWithEmployeeAsync(id);
            if (driver == null)
                return NotFound();

            var allEmployees = await _employeeService.GetEmployeesByPositionAsync(EmployeePosition.Driver);
            var allDrivers = await _driverService.GetAllDriversAsync();
            var driverEmployeeIds = allDrivers.Where(d => d.Id != id).Select(d => d.EmployeeId).ToHashSet();
            var availableEmployees = allEmployees.Where(e => !driverEmployeeIds.Contains(e.Id)).ToList();

            var model = new DriverViewModel
            {
                Id = driver.Id,
                EmployeeId = driver.EmployeeId,
                LicenseNumber = driver.LicenseNumber,
                LicenseExpiry = driver.LicenseExpiry,
                Status = driver.Status,
                EmployeeName = driver.Employee.Name,
                AvailableEmployees = new SelectList(availableEmployees, "Id", "Name", driver.EmployeeId)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DriverViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var driver = new Driver
                    {
                        Id = model.Id,
                        EmployeeId = model.EmployeeId,
                        LicenseNumber = model.LicenseNumber,
                        LicenseExpiry = model.LicenseExpiry,
                        Status = model.Status
                    };

                    await _driverService.UpdateDriverAsync(driver);
                    TempData["SuccessMessage"] = "Driver updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error updating driver {Id}", id);
                    if (ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true ||
                        ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        ModelState.AddModelError("LicenseNumber", "A driver with this license number already exists.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "A database error occurred while updating the driver.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating driver {Id}", id);
                    TempData["ErrorMessage"] = "An unexpected error occurred while updating the driver.";
                }
            }

            var allEmployees = await _employeeService.GetEmployeesByPositionAsync(EmployeePosition.Driver);
            var allDrivers = await _driverService.GetAllDriversAsync();
            var driverEmployeeIds = allDrivers.Where(d => d.Id != id).Select(d => d.EmployeeId).ToHashSet();
            var availableEmployees = allEmployees.Where(e => !driverEmployeeIds.Contains(e.Id)).ToList();
            model.AvailableEmployees = new SelectList(availableEmployees, "Id", "Name", model.EmployeeId);

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var driver = await _driverService.GetDriverWithEmployeeAsync(id);
            if (driver == null)
                return NotFound();

            return View(driver);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var driver = await _driverService.GetDriverWithEmployeeAsync(id);
            if (driver == null)
                return NotFound();

            var tripCount = await _driverService.GetTripCountAsync(id);
            ViewBag.TripCount = tripCount;

            return View(driver);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var canDelete = await _driverService.CanDeleteAsync(id);
                if (!canDelete)
                {
                    TempData["ErrorMessage"] = "Cannot delete this driver because they have associated trips. Please remove or reassign the trips first.";
                    return RedirectToAction(nameof(Index));
                }

                await _driverService.DeleteDriverAsync(id);
                TempData["SuccessMessage"] = "Driver deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting driver {Id}", id);
                TempData["ErrorMessage"] = "Cannot delete this driver because they have related records.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting driver {Id}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the driver.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Performance(int id)
        {
            try
            {
                var model = await _driverRatingService.GetDriverPerformanceAsync(id);
                return View(model);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading driver performance for driver {DriverId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading driver performance.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
