using Cab_Management_System.Models.ViewModels;
using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Cab_Management_System.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cab_Management_System.Areas.CustomerPortal.Controllers
{
    [Area("CustomerPortal")]
    [Authorize(Roles = "Customer")]
    public class DashboardController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ITripService _tripService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            ICustomerService customerService,
            ITripService tripService,
            UserManager<ApplicationUser> userManager,
            ILogger<DashboardController> logger)
        {
            _customerService = customerService;
            _tripService = tripService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account", new { area = "" });

                var customer = await _customerService.GetCustomerByEmailAsync(user.Email!);
                if (customer == null) return RedirectToAction("Login", "Account", new { area = "" });

                var trips = (await _tripService.GetTripsByCustomerIdAsync(customer.Id)).ToList();

                var model = new CustomerDashboardViewModel
                {
                    CustomerName = customer.Name,
                    TotalTrips = await _tripService.GetTripCountByCustomerIdAsync(customer.Id),
                    ActiveTrips = await _tripService.GetActiveTripCountByCustomerIdAsync(customer.Id),
                    TotalSpent = await _tripService.GetTotalSpentByCustomerIdAsync(customer.Id),
                    RecentTrips = trips.Take(5).ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer dashboard");
                return View(new CustomerDashboardViewModel());
            }
        }
    }
}
