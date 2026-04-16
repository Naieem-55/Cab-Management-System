using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;
using CabManagementSystem.Models.ViewModels;
using CabManagementSystem.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CabManagementSystem.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IBillingRepository _billingRepository;
        private readonly IMaintenanceRepository _maintenanceRepository;
        private readonly ITripFeedbackRepository _feedbackRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminDashboardService> _logger;

        public AdminDashboardService(
            IEmployeeRepository employeeRepository,
            IDriverRepository driverRepository,
            IVehicleRepository vehicleRepository,
            IRouteRepository routeRepository,
            ITripRepository tripRepository,
            IBillingRepository billingRepository,
            IMaintenanceRepository maintenanceRepository,
            ITripFeedbackRepository feedbackRepository,
            UserManager<ApplicationUser> userManager,
            ILogger<AdminDashboardService> logger)
        {
            _employeeRepository = employeeRepository;
            _driverRepository = driverRepository;
            _vehicleRepository = vehicleRepository;
            _routeRepository = routeRepository;
            _tripRepository = tripRepository;
            _billingRepository = billingRepository;
            _maintenanceRepository = maintenanceRepository;
            _feedbackRepository = feedbackRepository;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<AdminDashboardViewModel> GetAdminDashboardAsync()
        {
            _logger.LogInformation("Fetching Admin dashboard data");
            var recentTrips = await _tripRepository.GetAllAsync();
            var upcomingMaintenance = await _maintenanceRepository.FindAsync(
                m => m.Status == MaintenanceStatus.Scheduled);

            // Chart data - Trip status distribution
            var allTrips = recentTrips.ToList();
            var tripStatusGroups = allTrips.GroupBy(t => t.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .OrderBy(g => g.Status).ToList();

            // Chart data - Monthly revenue (last 6 months)
            var allBillings = (await _billingRepository.GetAllAsync()).ToList();
            var monthlyRevenue = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .Reverse()
                .Select(date => new
                {
                    Label = date.ToString("MMM yyyy"),
                    Revenue = allBillings
                        .Where(b => b.Status == PaymentStatus.Completed &&
                                    b.PaymentDate.Year == date.Year &&
                                    b.PaymentDate.Month == date.Month)
                        .Sum(b => b.Amount)
                }).ToList();

            // License expiry alerts
            var expiringDrivers = await _driverRepository.GetDriversWithExpiringLicensesAsync(30);

            return new AdminDashboardViewModel
            {
                TotalVehicles = await _vehicleRepository.CountAsync(),
                AvailableVehicles = await _vehicleRepository.CountAsync(v => v.Status == VehicleStatus.Available),
                TotalDrivers = await _driverRepository.CountAsync(),
                AvailableDrivers = await _driverRepository.CountAsync(d => d.Status == DriverStatus.Available),
                TotalEmployees = await _employeeRepository.CountAsync(),
                TotalRoutes = await _routeRepository.CountAsync(),
                ActiveTrips = await _tripRepository.CountAsync(t => t.Status == TripStatus.InProgress),
                TotalUsers = _userManager.Users.Count(),
                TotalRevenue = await _billingRepository.GetTotalRevenueAsync(),
                RecentTrips = allTrips.Take(5),
                UpcomingMaintenance = upcomingMaintenance.Take(5),
                TripStatusLabels = tripStatusGroups.Select(g => g.Status).ToList(),
                TripStatusCounts = tripStatusGroups.Select(g => g.Count).ToList(),
                MonthlyRevenueLabels = monthlyRevenue.Select(m => m.Label).ToList(),
                MonthlyRevenueData = monthlyRevenue.Select(m => m.Revenue).ToList(),
                ExpiringLicenses = expiringDrivers,
                ExpiringLicenseCount = expiringDrivers.Count(),
                TotalFeedback = await _feedbackRepository.CountAsync(),
                OpenComplaints = await _feedbackRepository.GetOpenFeedbackCountAsync(),
                AverageSatisfaction = await _feedbackRepository.GetAverageRatingAsync()
            };
        }
    }
}
