using CabManagementSystem.Models.Enums;
using CabManagementSystem.Models.ViewModels;
using CabManagementSystem.Repositories;
using Microsoft.Extensions.Logging;

namespace CabManagementSystem.Services
{
    public class HRDashboardService : IHRDashboardService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IDriverRatingRepository _driverRatingRepository;
        private readonly ILogger<HRDashboardService> _logger;

        public HRDashboardService(
            IEmployeeRepository employeeRepository,
            IDriverRepository driverRepository,
            IDriverRatingRepository driverRatingRepository,
            ILogger<HRDashboardService> logger)
        {
            _employeeRepository = employeeRepository;
            _driverRepository = driverRepository;
            _driverRatingRepository = driverRatingRepository;
            _logger = logger;
        }

        public async Task<HRDashboardViewModel> GetHRDashboardAsync()
        {
            _logger.LogInformation("Fetching HR dashboard data");
            var allEmployees = (await _employeeRepository.GetAllAsync()).ToList();
            var allDrivers = (await _driverRepository.GetAllAsync()).ToList();

            // Chart data - Position distribution
            var positionGroups = allEmployees.GroupBy(e => e.Position)
                .Select(g => new { Position = g.Key.ToString(), Count = g.Count() })
                .OrderBy(g => g.Position).ToList();

            // Chart data - Driver status distribution
            var driverStatusGroups = allDrivers.GroupBy(d => d.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .OrderBy(g => g.Status).ToList();

            // License expiry alerts
            var expiringDrivers = await _driverRepository.GetDriversWithExpiringLicensesAsync(30);

            // Top rated drivers (single batch query instead of N+1)
            var topRatedIds = await _driverRatingRepository.GetTopRatedDriverIdsAsync(5);
            var topRatedDrivers = topRatedIds
                .Select(x => (Driver: allDrivers.FirstOrDefault(d => d.Id == x.DriverId)!, AvgRating: x.AvgRating))
                .Where(x => x.Driver != null)
                .ToList();

            return new HRDashboardViewModel
            {
                TotalEmployees = allEmployees.Count,
                ActiveEmployees = allEmployees.Count(e => e.Status == EmployeeStatus.Active),
                TotalDrivers = allDrivers.Count,
                AvailableDrivers = allDrivers.Count(d => d.Status == DriverStatus.Available),
                EmployeesOnLeave = allEmployees.Count(e => e.Status == EmployeeStatus.OnLeave),
                RecentEmployees = allEmployees
                    .OrderByDescending(e => e.HireDate)
                    .Take(5),
                TopRatedDrivers = topRatedDrivers,
                PositionLabels = positionGroups.Select(g => g.Position).ToList(),
                PositionCounts = positionGroups.Select(g => g.Count).ToList(),
                DriverStatusLabels = driverStatusGroups.Select(g => g.Status).ToList(),
                DriverStatusCounts = driverStatusGroups.Select(g => g.Count).ToList(),
                ExpiringLicenses = expiringDrivers,
                ExpiringLicenseCount = expiringDrivers.Count()
            };
        }
    }
}
