using CabManagementSystem.Models.Enums;
using CabManagementSystem.Models.ViewModels;
using CabManagementSystem.Repositories;
using Microsoft.Extensions.Logging;

namespace CabManagementSystem.Services
{
    public class TravelDashboardService : ITravelDashboardService
    {
        private readonly ITripRepository _tripRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMaintenanceRepository _maintenanceRepository;
        private readonly ILogger<TravelDashboardService> _logger;

        public TravelDashboardService(
            ITripRepository tripRepository,
            IVehicleRepository vehicleRepository,
            IMaintenanceRepository maintenanceRepository,
            ILogger<TravelDashboardService> logger)
        {
            _tripRepository = tripRepository;
            _vehicleRepository = vehicleRepository;
            _maintenanceRepository = maintenanceRepository;
            _logger = logger;
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
