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
    public class TripController : Controller
    {
        private readonly ITripService _tripService;
        private readonly IDriverService _driverService;
        private readonly IVehicleService _vehicleService;
        private readonly IRouteService _routeService;
        private readonly ILogger<TripController> _logger;

        public TripController(
            ITripService tripService,
            IDriverService driverService,
            IVehicleService vehicleService,
            IRouteService routeService,
            ILogger<TripController> logger)
        {
            _tripService = tripService;
            _driverService = driverService;
            _vehicleService = vehicleService;
            _routeService = routeService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? searchTerm, TripStatus? status, int page = 1)
        {
            IEnumerable<Trip> trips;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                trips = await _tripService.SearchTripsAsync(searchTerm.Trim());
                ViewData["SearchTerm"] = searchTerm;
            }
            else
            {
                trips = await _tripService.GetAllTripsAsync();
            }

            if (status.HasValue)
            {
                trips = trips.Where(t => t.Status == status.Value);
                ViewData["SelectedStatus"] = status.Value.ToString();
            }

            var pageSize = 10;
            var paginatedList = PaginatedList<Trip>.Create(trips, page, pageSize);

            ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(TripStatus))
                .Cast<TripStatus>()
                .Select(s => new { Value = (int)s, Text = s.ToString() }),
                "Value", "Text");
            ViewBag.PageIndex = paginatedList.PageIndex;
            ViewBag.TotalPages = paginatedList.TotalPages;
            ViewBag.TotalCount = paginatedList.TotalCount;
            ViewBag.BaseUrl = Url.Action("Index");

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(searchTerm)) queryParams.Add($"&searchTerm={searchTerm}");
            if (status.HasValue) queryParams.Add($"&status={status.Value}");
            ViewBag.QueryString = string.Join("", queryParams);

            return View(paginatedList);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new TripViewModel
            {
                BookingDate = DateTime.Now,
                TripDate = DateTime.Now.AddDays(1)
            };

            await PopulateDropdownsAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TripViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var trip = new Trip
                    {
                        DriverId = model.DriverId,
                        VehicleId = model.VehicleId,
                        RouteId = model.RouteId,
                        CustomerName = model.CustomerName,
                        CustomerPhone = model.CustomerPhone,
                        CustomerEmail = model.CustomerEmail,
                        BookingDate = model.BookingDate,
                        TripDate = model.TripDate,
                        Status = model.Status,
                        Cost = model.Cost
                    };

                    await _tripService.CreateTripAsync(trip);
                    TempData["SuccessMessage"] = "Trip created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error creating trip");
                    TempData["ErrorMessage"] = "A database error occurred while creating the trip.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating trip");
                    TempData["ErrorMessage"] = "An unexpected error occurred while creating the trip.";
                }
            }

            await PopulateDropdownsAsync(model);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var trip = await _tripService.GetTripWithDetailsAsync(id);
            if (trip == null)
                return NotFound();

            var model = new TripViewModel
            {
                Id = trip.Id,
                DriverId = trip.DriverId,
                VehicleId = trip.VehicleId,
                RouteId = trip.RouteId,
                CustomerName = trip.CustomerName,
                CustomerPhone = trip.CustomerPhone,
                CustomerEmail = trip.CustomerEmail,
                BookingDate = trip.BookingDate,
                TripDate = trip.TripDate,
                Status = trip.Status,
                Cost = trip.Cost
            };

            await PopulateDropdownsAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TripViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTrip = await _tripService.GetTripByIdAsync(id);
                    if (existingTrip == null)
                        return NotFound();

                    var oldStatus = existingTrip.Status;
                    existingTrip.DriverId = model.DriverId;
                    existingTrip.VehicleId = model.VehicleId;
                    existingTrip.RouteId = model.RouteId;
                    existingTrip.CustomerName = model.CustomerName;
                    existingTrip.CustomerPhone = model.CustomerPhone;
                    existingTrip.CustomerEmail = model.CustomerEmail;
                    existingTrip.BookingDate = model.BookingDate;
                    existingTrip.TripDate = model.TripDate;
                    existingTrip.Cost = model.Cost;

                    if (oldStatus != model.Status)
                    {
                        await _tripService.UpdateTripStatusAsync(id, model.Status);
                    }
                    else
                    {
                        await _tripService.UpdateTripAsync(existingTrip);
                    }

                    TempData["SuccessMessage"] = "Trip updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error updating trip {Id}", id);
                    TempData["ErrorMessage"] = "A database error occurred while updating the trip.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating trip {Id}", id);
                    TempData["ErrorMessage"] = "An unexpected error occurred while updating the trip.";
                }
            }

            await PopulateDropdownsAsync(model);
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var trip = await _tripService.GetTripWithDetailsAsync(id);
            if (trip == null)
                return NotFound();

            return View(trip);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var trip = await _tripService.GetTripWithDetailsAsync(id);
            if (trip == null)
                return NotFound();

            return View(trip);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _tripService.DeleteTripAsync(id);
                TempData["SuccessMessage"] = "Trip deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting trip {Id}", id);
                TempData["ErrorMessage"] = "Cannot delete this trip because it has related billing records. Please remove the billing record first.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting trip {Id}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the trip.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Export()
        {
            var trips = await _tripService.GetAllTripsAsync();
            var columns = new Dictionary<string, Func<Trip, object>>
            {
                { "Customer", t => t.CustomerName },
                { "Driver", t => t.Driver?.Employee?.Name ?? "" },
                { "Vehicle", t => t.Vehicle?.RegistrationNumber ?? "" },
                { "Route", t => t.Route != null ? $"{t.Route.Origin} - {t.Route.Destination}" : "" },
                { "Trip Date", t => t.TripDate.ToString("yyyy-MM-dd") },
                { "Status", t => t.Status },
                { "Cost", t => t.Cost }
            };

            var csvData = Helpers.CsvExportHelper.ExportToCsv(trips, columns);
            return File(csvData, "text/csv", $"Trips_{DateTime.Now:yyyyMMdd}.csv");
        }

        private async Task PopulateDropdownsAsync(TripViewModel model)
        {
            var drivers = await _driverService.GetAvailableDriversAsync();
            model.AvailableDrivers = new SelectList(drivers, "Id", "Employee.Name");

            var vehicles = await _vehicleService.GetAvailableVehiclesAsync();
            model.AvailableVehicles = new SelectList(vehicles, "Id", "RegistrationNumber");

            var routes = await _routeService.GetAllRoutesAsync();
            var routeItems = routes.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = $"{r.Origin} \u2192 {r.Destination} ({r.Distance} km)"
            });
            model.AvailableRoutes = new SelectList(routeItems, "Value", "Text");
        }
    }
}
