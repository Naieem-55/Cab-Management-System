namespace CabManagementSystem.Models.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public string CustomerName { get; set; } = string.Empty;
        public int TotalTrips { get; set; }
        public int ActiveTrips { get; set; }
        public decimal TotalSpent { get; set; }
        public int LoyaltyPoints { get; set; }
        public List<Trip> RecentTrips { get; set; } = new();

        // Monthly activity (last 6 months)
        public List<string> MonthlyLabels { get; set; } = new();
        public List<int> MonthlyTripCounts { get; set; } = new();
        public List<decimal> MonthlySpendData { get; set; } = new();
    }
}
