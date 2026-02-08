using Cab_Management_System.Models;

namespace Cab_Management_System.Models.ViewModels
{
    public class FinanceDashboardViewModel
    {
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int PendingPayments { get; set; }
        public int CompletedPayments { get; set; }
        public int TotalBillings { get; set; }
        public IEnumerable<Billing> RecentBillings { get; set; } = new List<Billing>();

        // Expense data
        public decimal TotalExpenses { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public decimal NetProfit { get; set; }

        // Chart data
        public List<string> PaymentMethodLabels { get; set; } = new();
        public List<decimal> PaymentMethodAmounts { get; set; } = new();
        public List<string> RevenueTrendLabels { get; set; } = new();
        public List<decimal> RevenueTrendData { get; set; } = new();
        public List<string> ExpenseCategoryLabels { get; set; } = new();
        public List<decimal> ExpenseCategoryAmounts { get; set; } = new();
    }
}
