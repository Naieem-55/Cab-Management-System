using Cab_Management_System.Models;
using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cab_Management_System.Areas.CustomerPortal.Controllers
{
    [Area("CustomerPortal")]
    [Authorize(Roles = "Customer")]
    public class ProfileController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            ICustomerService customerService,
            UserManager<ApplicationUser> userManager,
            ILogger<ProfileController> logger)
        {
            _customerService = customerService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var customer = await GetCurrentCustomerAsync();
                if (customer == null) return RedirectToAction("Login", "Account", new { area = "" });

                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer profile");
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            try
            {
                var customer = await GetCurrentCustomerAsync();
                if (customer == null) return RedirectToAction("Login", "Account", new { area = "" });

                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer edit profile");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Customer model)
        {
            try
            {
                var customer = await GetCurrentCustomerAsync();
                if (customer == null) return RedirectToAction("Login", "Account", new { area = "" });

                ModelState.Remove("Email");
                ModelState.Remove("Trips");

                if (!ModelState.IsValid)
                    return View(model);

                customer.Name = model.Name;
                customer.Phone = model.Phone;
                customer.Address = model.Address;

                await _customerService.UpdateCustomerAsync(customer);

                // Also update ApplicationUser name
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var nameParts = model.Name.Trim().Split(' ', 2);
                    user.FirstName = nameParts[0];
                    user.LastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;
                    await _userManager.UpdateAsync(user);
                }

                TempData["SuccessMessage"] = "Profile updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer profile");
                ModelState.AddModelError(string.Empty, "An error occurred while updating your profile.");
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
