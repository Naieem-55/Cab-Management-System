namespace Cab_Management_System.Models.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public string CustomerName { get; set; } = string.Empty;
        public int TotalTrips { get; set; }
        public int ActiveTrips { get; set; }
        public decimal TotalSpent { get; set; }
        public List<Trip> RecentTrips { get; set; } = new();
    }
}
