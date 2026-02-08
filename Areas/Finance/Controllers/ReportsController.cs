using Cab_Management_System.Models;
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
        private readonly ITripService _tripService;
        private readonly IExpenseService _expenseService;

        public ReportsController(IBillingService billingService, ITripService tripService, IExpenseService expenseService)
        {
            _billingService = billingService;
            _tripService = tripService;
            _expenseService = expenseService;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue)
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var allBillings = await _billingService.GetAllBillingsAsync();
            var filteredBillings = allBillings.Where(b =>
                b.PaymentDate >= startDate.Value && b.PaymentDate <= endDate.Value).ToList();

            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");

            ViewBag.TotalRevenue = filteredBillings
                .Where(b => b.Status == PaymentStatus.Completed)
                .Sum(b => b.Amount);
            ViewBag.PendingAmount = filteredBillings
                .Where(b => b.Status == PaymentStatus.Pending)
                .Sum(b => b.Amount);
            ViewBag.CompletedPayments = filteredBillings
                .Count(b => b.Status == PaymentStatus.Completed);
            ViewBag.PendingPayments = filteredBillings
                .Count(b => b.Status == PaymentStatus.Pending);

            // Payment method breakdown
            var paymentBreakdown = filteredBillings
                .Where(b => b.Status == PaymentStatus.Completed)
                .GroupBy(b => b.PaymentMethod)
                .Select(g => new { Method = g.Key.ToString(), Count = g.Count(), Total = g.Sum(b => b.Amount) })
                .OrderByDescending(g => g.Total)
                .ToList();

            ViewBag.PaymentBreakdownMethods = paymentBreakdown.Select(p => p.Method).ToList();
            ViewBag.PaymentBreakdownCounts = paymentBreakdown.Select(p => p.Count).ToList();
            ViewBag.PaymentBreakdownTotals = paymentBreakdown.Select(p => p.Total).ToList();

            // Monthly revenue chart data (last 6 months)
            var monthlyData = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .Reverse()
                .Select(date => new
                {
                    Label = date.ToString("MMM yyyy"),
                    Revenue = allBillings
                        .Where(b => b.Status == PaymentStatus.Completed &&
                                    b.PaymentDate.Year == date.Year &&
                                    b.PaymentDate.Month == date.Month)
                        .Sum(b => b.Amount)
                }).ToList();

            ViewBag.MonthlyLabels = monthlyData.Select(m => m.Label).ToList();
            ViewBag.MonthlyRevenue = monthlyData.Select(m => m.Revenue).ToList();

            // Expense data
            var allExpenses = await _expenseService.GetAllExpensesAsync();
            var filteredExpenses = allExpenses.Where(e =>
                e.Date >= startDate.Value && e.Date <= endDate.Value).ToList();

            var totalExpenses = filteredExpenses.Sum(e => e.Amount);
            var totalRevenue = filteredBillings
                .Where(b => b.Status == PaymentStatus.Completed)
                .Sum(b => b.Amount);

            ViewBag.TotalExpenses = totalExpenses;
            ViewBag.NetProfit = totalRevenue - totalExpenses;

            return View(filteredBillings);
        }
    }
}
