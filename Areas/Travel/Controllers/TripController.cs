using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Models.ViewModels;
using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public TripController(
            ITripService tripService,
            IDriverService driverService,
            IVehicleService vehicleService,
            IRouteService routeService)
        {
            _tripService = tripService;
            _driverService = driverService;
            _vehicleService = vehicleService;
            _routeService = routeService;
        }

        public async Task<IActionResult> Index(string? searchTerm, TripStatus? status)
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

            ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(TripStatus))
                .Cast<TripStatus>()
                .Select(s => new { Value = (int)s, Text = s.ToString() }),
                "Value", "Text");

            return View(trips);
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
                var trip = new Trip
                {
                    Id = model.Id,
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

                await _tripService.UpdateTripAsync(trip);
                TempData["SuccessMessage"] = "Trip updated successfully.";
                return RedirectToAction(nameof(Index));
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
            await _tripService.DeleteTripAsync(id);
            TempData["SuccessMessage"] = "Trip deleted successfully.";
            return RedirectToAction(nameof(Index));
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
