using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Models.ViewModels;
using Cab_Management_System.Repositories;

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

        public DashboardService(
            IEmployeeRepository employeeRepository,
            IDriverRepository driverRepository,
            IVehicleRepository vehicleRepository,
            IRouteRepository routeRepository,
            ITripRepository tripRepository,
            IBillingRepository billingRepository,
            IMaintenanceRepository maintenanceRepository)
        {
            _employeeRepository = employeeRepository;
            _driverRepository = driverRepository;
            _vehicleRepository = vehicleRepository;
            _routeRepository = routeRepository;
            _tripRepository = tripRepository;
            _billingRepository = billingRepository;
            _maintenanceRepository = maintenanceRepository;
        }

        public async Task<AdminDashboardViewModel> GetAdminDashboardAsync()
        {
            var recentTrips = await _tripRepository.GetAllAsync();
            var upcomingMaintenance = await _maintenanceRepository.FindAsync(
                m => m.Status == MaintenanceStatus.Scheduled);

            return new AdminDashboardViewModel
            {
                TotalVehicles = await _vehicleRepository.CountAsync(),
                AvailableVehicles = await _vehicleRepository.CountAsync(v => v.Status == VehicleStatus.Available),
                TotalDrivers = await _driverRepository.CountAsync(),
                AvailableDrivers = await _driverRepository.CountAsync(d => d.Status == DriverStatus.Available),
                TotalEmployees = await _employeeRepository.CountAsync(),
                TotalRoutes = await _routeRepository.CountAsync(),
                ActiveTrips = await _tripRepository.CountAsync(t => t.Status == TripStatus.InProgress),
                TotalUsers = 0,
                TotalRevenue = await _billingRepository.GetTotalRevenueAsync(),
                RecentTrips = recentTrips.Take(5),
                UpcomingMaintenance = upcomingMaintenance.Take(5)
            };
        }

        public async Task<FinanceDashboardViewModel> GetFinanceDashboardAsync()
        {
            var recentBillings = await _billingRepository.GetAllAsync();
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            var monthlyBillings = await _billingRepository.GetBillingsByDateRangeAsync(startOfMonth, endOfMonth);

            return new FinanceDashboardViewModel
            {
                TotalRevenue = await _billingRepository.GetTotalRevenueAsync(),
                PendingPayments = await _billingRepository.CountAsync(b => b.Status == PaymentStatus.Pending),
                CompletedPayments = await _billingRepository.CountAsync(b => b.Status == PaymentStatus.Completed),
                TotalBillings = await _billingRepository.CountAsync(),
                RecentBillings = recentBillings.Take(5),
                MonthlyRevenue = monthlyBillings
                    .Where(b => b.Status == PaymentStatus.Completed)
                    .Sum(b => b.Amount)
            };
        }

        public async Task<HRDashboardViewModel> GetHRDashboardAsync()
        {
            var allEmployees = await _employeeRepository.GetAllAsync();

            return new HRDashboardViewModel
            {
                TotalEmployees = await _employeeRepository.CountAsync(),
                ActiveEmployees = await _employeeRepository.CountAsync(e => e.Status == EmployeeStatus.Active),
                TotalDrivers = await _driverRepository.CountAsync(),
                AvailableDrivers = await _driverRepository.CountAsync(d => d.Status == DriverStatus.Available),
                EmployeesOnLeave = await _employeeRepository.CountAsync(e => e.Status == EmployeeStatus.OnLeave),
                RecentEmployees = allEmployees
                    .OrderByDescending(e => e.HireDate)
                    .Take(5)
            };
        }

        public async Task<TravelDashboardViewModel> GetTravelDashboardAsync()
        {
            var recentTrips = await _tripRepository.GetAllAsync();
            var overdueMaintenance = await _maintenanceRepository.GetOverdueMaintenanceAsync();

            return new TravelDashboardViewModel
            {
                TotalTrips = await _tripRepository.CountAsync(),
                ActiveTrips = await _tripRepository.CountAsync(t => t.Status == TripStatus.InProgress),
                CompletedTrips = await _tripRepository.CountAsync(t => t.Status == TripStatus.Completed),
                TotalVehicles = await _vehicleRepository.CountAsync(),
                AvailableVehicles = await _vehicleRepository.CountAsync(v => v.Status == VehicleStatus.Available),
                OverdueMaintenance = overdueMaintenance.Count(),
                RecentTrips = recentTrips.Take(5)
            };
        }
    }
}
