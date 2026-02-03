using Cab_Management_System.Models;

namespace Cab_Management_System.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalVehicles { get; set; }
        public int AvailableVehicles { get; set; }
        public int TotalDrivers { get; set; }
        public int AvailableDrivers { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalRoutes { get; set; }
        public int ActiveTrips { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        public IEnumerable<Trip> RecentTrips { get; set; } = new List<Trip>();
        public IEnumerable<MaintenanceRecord> UpcomingMaintenance { get; set; } = new List<MaintenanceRecord>();
    }
}
