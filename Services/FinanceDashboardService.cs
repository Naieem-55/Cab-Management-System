using CabManagementSystem.Models.Enums;
using CabManagementSystem.Models.ViewModels;
using CabManagementSystem.Repositories;
using Microsoft.Extensions.Logging;

namespace CabManagementSystem.Services
{
    public class FinanceDashboardService : IFinanceDashboardService
    {
        private readonly IBillingRepository _billingRepository;
        private readonly IExpenseRepository _expenseRepository;
        private readonly ILogger<FinanceDashboardService> _logger;

        public FinanceDashboardService(
            IBillingRepository billingRepository,
            IExpenseRepository expenseRepository,
            ILogger<FinanceDashboardService> logger)
        {
            _billingRepository = billingRepository;
            _expenseRepository = expenseRepository;
            _logger = logger;
        }

        public async Task<FinanceDashboardViewModel> GetFinanceDashboardAsync()
        {
            _logger.LogInformation("Fetching Finance dashboard data");
            var recentBillings = await _billingRepository.GetAllAsync();
            var billingsList = recentBillings.ToList();
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            var monthlyBillings = await _billingRepository.GetBillingsByDateRangeAsync(startOfMonth, endOfMonth);

            // Chart data - Payment method distribution
            var paymentMethodGroups = billingsList
                .Where(b => b.Status == PaymentStatus.Completed)
                .GroupBy(b => b.PaymentMethod)
                .Select(g => new { Method = g.Key.ToString(), Amount = g.Sum(b => b.Amount) })
                .OrderBy(g => g.Method).ToList();

            // Chart data - Revenue trend (last 6 months)
            var revenueTrend = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .Reverse()
                .Select(date => new
                {
                    Label = date.ToString("MMM yyyy"),
                    Revenue = billingsList
                        .Where(b => b.Status == PaymentStatus.Completed &&
                                    b.PaymentDate.Year == date.Year &&
                                    b.PaymentDate.Month == date.Month)
                        .Sum(b => b.Amount)
                }).ToList();

            // Expense data
            var totalExpenses = await _expenseRepository.GetTotalExpensesAsync();
            var monthlyExpenses = await _expenseRepository.GetTotalExpensesByDateRangeAsync(startOfMonth, endOfMonth);
            var totalRevenue = await _billingRepository.GetTotalRevenueAsync();

            // Expense category grouping
            var allExpenses = (await _expenseRepository.GetAllAsync()).ToList();
            var categoryGroups = allExpenses
                .GroupBy(e => e.Category)
                .Select(g => new { Category = g.Key.ToString(), Amount = g.Sum(e => e.Amount) })
                .OrderByDescending(g => g.Amount).ToList();

            return new FinanceDashboardViewModel
            {
                TotalRevenue = totalRevenue,
                PendingPayments = await _billingRepository.CountAsync(b => b.Status == PaymentStatus.Pending),
                CompletedPayments = await _billingRepository.CountAsync(b => b.Status == PaymentStatus.Completed),
                TotalBillings = await _billingRepository.CountAsync(),
                RecentBillings = billingsList.Take(5),
                MonthlyRevenue = monthlyBillings
                    .Where(b => b.Status == PaymentStatus.Completed)
                    .Sum(b => b.Amount),
                TotalExpenses = totalExpenses,
                MonthlyExpenses = monthlyExpenses,
                NetProfit = totalRevenue - totalExpenses,
                PaymentMethodLabels = paymentMethodGroups.Select(g => g.Method).ToList(),
                PaymentMethodAmounts = paymentMethodGroups.Select(g => g.Amount).ToList(),
                RevenueTrendLabels = revenueTrend.Select(r => r.Label).ToList(),
                RevenueTrendData = revenueTrend.Select(r => r.Revenue).ToList(),
                ExpenseCategoryLabels = categoryGroups.Select(g => g.Category).ToList(),
                ExpenseCategoryAmounts = categoryGroups.Select(g => g.Amount).ToList()
            };
        }
    }
}
