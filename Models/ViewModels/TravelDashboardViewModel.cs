using Cab_Management_System.Models;

namespace Cab_Management_System.Models.ViewModels
{
    public class TravelDashboardViewModel
    {
        public int TotalTrips { get; set; }
        public int ActiveTrips { get; set; }
        public int CompletedTrips { get; set; }
        public int TotalVehicles { get; set; }
        public int AvailableVehicles { get; set; }
        public int OverdueMaintenance { get; set; }
        public IEnumerable<Trip> RecentTrips { get; set; } = new List<Trip>();

        // Chart data
        public List<string> TripStatusLabels { get; set; } = new();
        public List<int> TripStatusCounts { get; set; } = new();
        public List<string> VehicleStatusLabels { get; set; } = new();
        public List<int> VehicleStatusCounts { get; set; } = new();
    }
}
