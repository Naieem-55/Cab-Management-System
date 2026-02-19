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
        private readonly IDriverRatingService _driverRatingService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TripController> _logger;

        public TripController(
            ICustomerService customerService,
            ITripService tripService,
            IRouteService routeService,
            IDriverService driverService,
            IVehicleService vehicleService,
            IDriverRatingService driverRatingService,
            UserManager<ApplicationUser> userManager,
            ILogger<TripController> logger)
        {
            _customerService = customerService;
            _tripService = tripService;
            _routeService = routeService;
            _driverService = driverService;
            _vehicleService = vehicleService;
            _driverRatingService = driverRatingService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? searchTerm, TripStatus? status, int page = 1)
        {
            try
            {
                var customer = await GetCurrentCustomerAsync();
                if (customer == null) return RedirectToAction("Login", "Account", new { area = "" });

                var trips = await _tripService.GetTripsByCustomerIdAsync(customer.Id);

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    trips = trips.Where(t =>
                        (t.Route?.Origin?.ToLower().Contains(term) == true) ||
                        (t.Route?.Destination?.ToLower().Contains(term) == true));
                    ViewData["SearchTerm"] = searchTerm;
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer trips");
                return View(PaginatedList<Trip>.Create(Enumerable.Empty<Trip>(), 1, 10));
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
                var drivers = await _driverService.GetAvailableDriversAsync();
                var availableDriver = drivers.FirstOrDefault();
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
                var vehicles = await _vehicleService.GetAvailableVehiclesAsync();
                var availableVehicle = vehicles.FirstOrDefault();
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

        [HttpGet]
        public async Task<IActionResult> Rate(int id)
        {
            try
            {
                var customer = await GetCurrentCustomerAsync();
                if (customer == null) return RedirectToAction("Login", "Account", new { area = "" });

                var trip = await _tripService.GetTripWithDetailsAsync(id);
                if (trip == null || trip.CustomerId != customer.Id)
                    return NotFound();

                if (trip.Status != TripStatus.Completed)
                {
                    TempData["ErrorMessage"] = "Only completed trips can be rated.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var canRate = await _driverRatingService.CanRateTripAsync(id);
                if (!canRate)
                {
                    TempData["ErrorMessage"] = "This trip has already been rated.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var model = new RateTripViewModel
                {
                    TripId = trip.Id,
                    DriverName = trip.Driver?.Employee?.Name ?? "Unknown",
                    RouteInfo = trip.Route != null ? $"{trip.Route.Origin} → {trip.Route.Destination}" : "N/A",
                    TripDate = trip.TripDate
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading rate form for trip {TripId}", id);
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rate(RateTripViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var customer = await GetCurrentCustomerAsync();
                    if (customer == null) return RedirectToAction("Login", "Account", new { area = "" });

                    var trip = await _tripService.GetTripWithDetailsAsync(model.TripId);
                    if (trip == null || trip.CustomerId != customer.Id)
                        return NotFound();

                    var canRate = await _driverRatingService.CanRateTripAsync(model.TripId);
                    if (!canRate)
                    {
                        TempData["ErrorMessage"] = "This trip cannot be rated.";
                        return RedirectToAction(nameof(Details), new { id = model.TripId });
                    }

                    var rating = new DriverRating
                    {
                        TripId = model.TripId,
                        DriverId = trip.DriverId,
                        Rating = model.Rating,
                        Comment = model.Comment,
                        CustomerName = customer.Name
                    };

                    await _driverRatingService.CreateRatingAsync(rating);
                    TempData["SuccessMessage"] = "Thank you for rating this trip!";
                    return RedirectToAction(nameof(Details), new { id = model.TripId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating rating for trip {TripId}", model.TripId);
                    TempData["ErrorMessage"] = "An error occurred while submitting the rating.";
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var customer = await GetCurrentCustomerAsync();
                if (customer == null) return RedirectToAction("Login", "Account", new { area = "" });

                var trip = await _tripService.GetTripWithDetailsAsync(id);
                if (trip == null || trip.CustomerId != customer.Id)
                    return NotFound();

                if (trip.Status != TripStatus.Pending && trip.Status != TripStatus.Confirmed)
                {
                    TempData["ErrorMessage"] = "Only pending or confirmed trips can be cancelled.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                await _tripService.UpdateTripStatusAsync(id, TripStatus.Cancelled);
                TempData["SuccessMessage"] = "Trip has been cancelled successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling trip {TripId}", id);
                TempData["ErrorMessage"] = "An error occurred while cancelling the trip.";
                return RedirectToAction(nameof(Details), new { id });
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
