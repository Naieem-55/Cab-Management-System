using Cab_Management_System.Models;
using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cab_Management_System.Areas.CustomerPortal.Controllers
{
    [Area("CustomerPortal")]
    [Authorize(Roles = "Customer")]
    public class InvoiceController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IBillingService _billingService;
        private readonly IInvoicePdfService _invoicePdfService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(
            ICustomerService customerService,
            IBillingService billingService,
            IInvoicePdfService invoicePdfService,
            UserManager<ApplicationUser> userManager,
            ILogger<InvoiceController> logger)
        {
            _customerService = customerService;
            _billingService = billingService;
            _invoicePdfService = invoicePdfService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var customer = await GetCurrentCustomerAsync();
                if (customer == null) return RedirectToAction("Login", "Account", new { area = "" });

                var billings = await _billingService.GetBillingsByCustomerIdAsync(customer.Id);
                return View(billings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer invoices");
                return View(Enumerable.Empty<Billing>());
            }
        }

        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var customer = await GetCurrentCustomerAsync();
                if (customer == null) return RedirectToAction("Login", "Account", new { area = "" });

                var billing = await _billingService.GetBillingWithTripAsync(id);
                if (billing == null || billing.Trip?.CustomerId != customer.Id)
                    return NotFound();

                var pdfBytes = _invoicePdfService.GenerateInvoice(billing);
                return File(pdfBytes, "application/pdf", $"Invoice_{billing.Id}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading invoice for billing ID {Id}", id);
                TempData["ErrorMessage"] = "Error generating invoice. Please try again.";
                return RedirectToAction(nameof(Index));
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
