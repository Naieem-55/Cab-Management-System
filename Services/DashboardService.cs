using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Models.ViewModels;
using Cab_Management_System.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Cab_Management_System.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IBillingRepository _billingRepository;
        private readonly IMaintenanceRepository _maintenanceRepository;
        private readonly IExpenseRepository _expenseRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            IEmployeeRepository employeeRepository,
            IDriverRepository driverRepository,
            IVehicleRepository vehicleRepository,
            IRouteRepository routeRepository,
            ITripRepository tripRepository,
            IBillingRepository billingRepository,
            IMaintenanceRepository maintenanceRepository,
            IExpenseRepository expenseRepository,
            UserManager<ApplicationUser> userManager,
            ILogger<DashboardService> logger)
        {
            _employeeRepository = employeeRepository;
            _driverRepository = driverRepository;
            _vehicleRepository = vehicleRepository;
            _routeRepository = routeRepository;
            _tripRepository = tripRepository;
            _billingRepository = billingRepository;
            _maintenanceRepository = maintenanceRepository;
            _expenseRepository = expenseRepository;
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
                ExpiringLicenseCount = expiringDrivers.Count()
            };
        }

        public async Task<FinanceDashboardViewModel> GetFinanceDashboardAsync()
        {
            _logger.LogInformation("Fetching Finance dashboard data");
            var recentBillings = await _billingRepository.GetAllAsync();
            var billingsList = recentBillings.ToList();
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            var monthlyBillings = await _billingRepository.GetBillingsByDateRangeAsync(startOfMonth, endOfMonth);

            // Chart data - Payment method distribution
            var paymentMethodGroups = billingsList
                .Where(b => b.Status == PaymentStatus.Completed)
                .GroupBy(b => b.PaymentMethod)
                .Select(g => new { Method = g.Key.ToString(), Amount = g.Sum(b => b.Amount) })
                .OrderBy(g => g.Method).ToList();

            // Chart data - Revenue trend (last 6 months)
            var revenueTrend = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .Reverse()
                .Select(date => new
                {
                    Label = date.ToString("MMM yyyy"),
                    Revenue = billingsList
                        .Where(b => b.Status == PaymentStatus.Completed &&
                                    b.PaymentDate.Year == date.Year &&
                                    b.PaymentDate.Month == date.Month)
                        .Sum(b => b.Amount)
                }).ToList();

            // Expense data
            var totalExpenses = await _expenseRepository.GetTotalExpensesAsync();
            var monthlyExpenses = await _expenseRepository.GetTotalExpensesByDateRangeAsync(startOfMonth, endOfMonth);
            var totalRevenue = await _billingRepository.GetTotalRevenueAsync();

            // Expense category grouping
            var allExpenses = (await _expenseRepository.GetAllAsync()).ToList();
            var categoryGroups = allExpenses
                .GroupBy(e => e.Category)
                .Select(g => new { Category = g.Key.ToString(), Amount = g.Sum(e => e.Amount) })
                .OrderByDescending(g => g.Amount).ToList();

            return new FinanceDashboardViewModel
            {
                TotalRevenue = totalRevenue,
                PendingPayments = await _billingRepository.CountAsync(b => b.Status == PaymentStatus.Pending),
                CompletedPayments = await _billingRepository.CountAsync(b => b.Status == PaymentStatus.Completed),
                TotalBillings = await _billingRepository.CountAsync(),
                RecentBillings = billingsList.Take(5),
                MonthlyRevenue = monthlyBillings
                    .Where(b => b.Status == PaymentStatus.Completed)
                    .Sum(b => b.Amount),
                TotalExpenses = totalExpenses,
                MonthlyExpenses = monthlyExpenses,
                NetProfit = totalRevenue - totalExpenses,
                PaymentMethodLabels = paymentMethodGroups.Select(g => g.Method).ToList(),
                PaymentMethodAmounts = paymentMethodGroups.Select(g => g.Amount).ToList(),
                RevenueTrendLabels = revenueTrend.Select(r => r.Label).ToList(),
                RevenueTrendData = revenueTrend.Select(r => r.Revenue).ToList(),
                ExpenseCategoryLabels = categoryGroups.Select(g => g.Category).ToList(),
                ExpenseCategoryAmounts = categoryGroups.Select(g => g.Amount).ToList()
            };
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
                PositionLabels = positionGroups.Select(g => g.Position).ToList(),
                PositionCounts = positionGroups.Select(g => g.Count).ToList(),
                DriverStatusLabels = driverStatusGroups.Select(g => g.Status).ToList(),
                DriverStatusCounts = driverStatusGroups.Select(g => g.Count).ToList(),
                ExpiringLicenses = expiringDrivers,
                ExpiringLicenseCount = expiringDrivers.Count()
            };
        }

        public async Task<TravelDashboardViewModel> GetTravelDashboardAsync()
        {
            _logger.LogInformation("Fetching Travel dashboard data");
            var recentTrips = (await _tripRepository.GetAllAsync()).ToList();
            var overdueMaintenance = await _maintenanceRepository.GetOverdueMaintenanceAsync();
            var allVehicles = (await _vehicleRepository.GetAllAsync()).ToList();

            // Chart data - Trip status distribution
            var tripStatusGroups = recentTrips.GroupBy(t => t.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .OrderBy(g => g.Status).ToList();

            // Chart data - Vehicle status distribution
            var vehicleStatusGroups = allVehicles.GroupBy(v => v.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .OrderBy(g => g.Status).ToList();

            return new TravelDashboardViewModel
            {
                TotalTrips = recentTrips.Count,
                ActiveTrips = recentTrips.Count(t => t.Status == TripStatus.InProgress),
                CompletedTrips = recentTrips.Count(t => t.Status == TripStatus.Completed),
                TotalVehicles = allVehicles.Count,
                AvailableVehicles = allVehicles.Count(v => v.Status == VehicleStatus.Available),
                OverdueMaintenance = overdueMaintenance.Count(),
                RecentTrips = recentTrips.Take(5),
                TripStatusLabels = tripStatusGroups.Select(g => g.Status).ToList(),
                TripStatusCounts = tripStatusGroups.Select(g => g.Count).ToList(),
                VehicleStatusLabels = vehicleStatusGroups.Select(g => g.Status).ToList(),
                VehicleStatusCounts = vehicleStatusGroups.Select(g => g.Count).ToList()
            };
        }
    }
}
