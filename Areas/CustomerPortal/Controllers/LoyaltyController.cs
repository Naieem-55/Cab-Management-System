using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;
using CabManagementSystem.Models.ViewModels;
using CabManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CabManagementSystem.Areas.CustomerPortal.Controllers
{
    [Area("CustomerPortal")]
    [Authorize(Roles = nameof(UserRole.Customer))]
    public class LoyaltyController : Controller
    {
        private readonly ILoyaltyPointsService _loyaltyService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoyaltyController> _logger;

        public LoyaltyController(
            ILoyaltyPointsService loyaltyService,
            ICustomerService customerService,
            UserManager<ApplicationUser> userManager,
            ILogger<LoyaltyController> logger)
        {
            _loyaltyService = loyaltyService;
            _customerService = customerService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account", new { area = "" });

            var balance = await _loyaltyService.GetBalanceAsync(customer.Id);
            var transactions = await _loyaltyService.GetHistoryAsync(customer.Id);

            var pageSize = 15;
            var paginatedList = PaginatedList<LoyaltyTransaction>.Create(transactions, page, pageSize);

            ViewBag.Balance = balance;
            ViewBag.PageIndex = paginatedList.PageIndex;
            ViewBag.TotalPages = paginatedList.TotalPages;
            ViewBag.TotalCount = paginatedList.TotalCount;
            ViewBag.BaseUrl = Url.Action("Index");
            ViewBag.QueryString = "";
            ViewBag.PointsToDollarRatio = LoyaltyPointsService.POINTS_TO_DOLLAR_RATIO;

            return View(paginatedList);
        }

        private async Task<Customer?> GetCurrentCustomerAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.Email == null) return null;
            return await _customerService.GetCustomerByEmailAsync(user.Email);
        }
    }
}
