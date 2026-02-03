using Cab_Management_System.Models.Enums;
using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cab_Management_System.Areas.Finance.Controllers
{
    [Area("Finance")]
    [Authorize(Roles = "FinanceManager")]
    public class ReportsController : Controller
    {
        private readonly IBillingService _billingService;

        public ReportsController(IBillingService billingService)
        {
            _billingService = billingService;
        }

        public async Task<IActionResult> Index()
        {
            var billings = await _billingService.GetAllBillingsAsync();
            var billingsList = billings.ToList();

            ViewBag.TotalRevenue = billingsList
                .Where(b => b.Status == PaymentStatus.Completed)
                .Sum(b => b.Amount);
            ViewBag.PendingAmount = billingsList
                .Where(b => b.Status == PaymentStatus.Pending)
                .Sum(b => b.Amount);
            ViewBag.CompletedPayments = billingsList
                .Count(b => b.Status == PaymentStatus.Completed);
            ViewBag.PendingPayments = billingsList
                .Count(b => b.Status == PaymentStatus.Pending);

            return View(billingsList);
        }
    }
}
