using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Models.ViewModels;
using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cab_Management_System.Areas.CustomerPortal.Controllers
{
    [Area("CustomerPortal")]
    [Authorize(Roles = "Customer")]
    public class TripController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ITripService _tripService;
        private readonly IRouteService _routeService;
        private readonly IDriverService _driverService;
        private readonly IVehicleService _vehicleService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TripController> _logger;

        public TripController(
            ICustomerService customerService,
            ITripService tripService,
            IRouteService routeService,
            IDriverService driverService,
            IVehicleService vehicleService,
            UserManager<ApplicationUser> userManager,
            ILogger<TripController> logger)
        {
            _customerService = customerService;
            _tripService = tripService;
            _routeService = routeService;
            _driverService = driverService;
            _vehicleService = vehicleService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var customer = await GetCurrentCustomerAsync();
                if (customer == null) return RedirectToAction("Login", "Account", new { area = "" });

                var trips = await _tripService.GetTripsByCustomerIdAsync(customer.Id);
                return View(trips);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer trips");
                return View(Enumerable.Empty<Trip>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var customer = await GetCurrentCustomerAsync();
                if (customer == null) return RedirectToAction("Login", "Account", new { area = "" });

                var trip = await _tripService.GetTripWithDetailsAsync(id);
                if (trip == null || trip.CustomerId != customer.Id)
                    return NotFound();

                return View(trip);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading trip details for ID {Id}", id);
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Book()
        {
            try
            {
                var routes = await _routeService.GetAllRoutesAsync();
                var model = new CustomerBookTripViewModel
                {
                    AvailableRoutes = new SelectList(
                        routes.Select(r => new { r.Id, Display = $"{r.Origin} → {r.Destination} ({r.Distance} km) - {r.BaseCost:C}" }),
                        "Id", "Display")
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading book trip form");
                return View(new CustomerBookTripViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(CustomerBookTripViewModel model)
        {
            try
            {
                var customer = await GetCurrentCustomerAsync();
                if (customer == null) return RedirectToAction("Login", "Account", new { area = "" });

                if (!ModelState.IsValid)
                {
                    var routes = await _routeService.GetAllRoutesAsync();
                    model.AvailableRoutes = new SelectList(
                        routes.Select(r => new { r.Id, Display = $"{r.Origin} → {r.Destination} ({r.Distance} km) - {r.BaseCost:C}" }),
                        "Id", "Display");
                    return View(model);
                }

                // Get route for cost
                var route = await _routeService.GetRouteByIdAsync(model.RouteId);
                if (route == null)
                {
                    ModelState.AddModelError("RouteId", "Selected route not found.");
                    var routes = await _routeService.GetAllRoutesAsync();
                    model.AvailableRoutes = new SelectList(
                        routes.Select(r => new { r.Id, Display = $"{r.Origin} → {r.Destination} ({r.Distance} km) - {r.BaseCost:C}" }),
                        "Id", "Display");
                    return View(model);
                }

                // Auto-assign available driver
                var drivers = await _driverService.GetAllDriversAsync();
                var availableDriver = drivers.FirstOrDefault(d => d.Status == DriverStatus.Available);
                if (availableDriver == null)
                {
                    ModelState.AddModelError(string.Empty, "No drivers are currently available. Please try again later.");
                    var routes = await _routeService.GetAllRoutesAsync();
                    model.AvailableRoutes = new SelectList(
                        routes.Select(r => new { r.Id, Display = $"{r.Origin} → {r.Destination} ({r.Distance} km) - {r.BaseCost:C}" }),
                        "Id", "Display");
                    return View(model);
                }

                // Auto-assign available vehicle
                var vehicles = await _vehicleService.GetAllVehiclesAsync();
                var availableVehicle = vehicles.FirstOrDefault(v => v.Status == VehicleStatus.Available);
                if (availableVehicle == null)
                {
                    ModelState.AddModelError(string.Empty, "No vehicles are currently available. Please try again later.");
                    var routes = await _routeService.GetAllRoutesAsync();
                    model.AvailableRoutes = new SelectList(
                        routes.Select(r => new { r.Id, Display = $"{r.Origin} → {r.Destination} ({r.Distance} km) - {r.BaseCost:C}" }),
                        "Id", "Display");
                    return View(model);
                }

                var trip = new Trip
                {
                    DriverId = availableDriver.Id,
                    VehicleId = availableVehicle.Id,
                    RouteId = model.RouteId,
                    CustomerName = customer.Name,
                    CustomerPhone = customer.Phone,
                    CustomerEmail = customer.Email,
                    CustomerId = customer.Id,
                    BookingDate = DateTime.Now,
                    TripDate = model.TripDate,
                    Status = TripStatus.Pending,
                    Cost = route.BaseCost
                };

                await _tripService.CreateTripAsync(trip);
                TempData["SuccessMessage"] = "Trip booked successfully! Your trip is pending confirmation.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error booking trip");
                ModelState.AddModelError(string.Empty, "An error occurred while booking your trip. Please try again.");
                var routes = await _routeService.GetAllRoutesAsync();
                model.AvailableRoutes = new SelectList(
                    routes.Select(r => new { r.Id, Display = $"{r.Origin} → {r.Destination} ({r.Distance} km) - {r.BaseCost:C}" }),
                    "Id", "Display");
                return View(model);
            }
        }

        private async Task<Customer?> GetCurrentCustomerAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.Email == null) return null;
            return await _customerService.GetCustomerByEmailAsync(user.Email);
        }
    }
}
