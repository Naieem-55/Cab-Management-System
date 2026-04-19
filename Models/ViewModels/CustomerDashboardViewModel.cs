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
    }
}
