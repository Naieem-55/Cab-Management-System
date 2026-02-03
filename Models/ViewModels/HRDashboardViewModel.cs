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
    }
}
