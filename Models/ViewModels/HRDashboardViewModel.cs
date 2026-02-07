using Cab_Management_System.Models;

namespace Cab_Management_System.Models.ViewModels
{
    public class HRDashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int TotalDrivers { get; set; }
        public int AvailableDrivers { get; set; }
        public int EmployeesOnLeave { get; set; }
        public IEnumerable<Employee> RecentEmployees { get; set; } = new List<Employee>();

        // Chart data
        public List<string> PositionLabels { get; set; } = new();
        public List<int> PositionCounts { get; set; } = new();
        public List<string> DriverStatusLabels { get; set; } = new();
        public List<int> DriverStatusCounts { get; set; } = new();
    }
}
