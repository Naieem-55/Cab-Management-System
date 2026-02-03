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
    }
}
