using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Repositories;
using Microsoft.Extensions.Logging;

namespace Cab_Management_System.Services
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ILogger<TripService> _logger;

        public TripService(
            ITripRepository tripRepository,
            IDriverRepository driverRepository,
            IVehicleRepository vehicleRepository,
            ILogger<TripService> logger)
        {
            _tripRepository = tripRepository;
            _driverRepository = driverRepository;
            _vehicleRepository = vehicleRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Trip>> GetAllTripsAsync()
            => await _tripRepository.GetAllAsync();

        public async Task<Trip?> GetTripByIdAsync(int id)
            => await _tripRepository.GetByIdAsync(id);

        public async Task<Trip?> GetTripWithDetailsAsync(int id)
            => await _tripRepository.GetTripWithDetailsAsync(id);

        public async Task<IEnumerable<Trip>> GetTripsByStatusAsync(TripStatus status)
            => await _tripRepository.GetTripsByStatusAsync(status);

        public async Task<IEnumerable<Trip>> GetTripsByDateRangeAsync(DateTime startDate, DateTime endDate)
            => await _tripRepository.GetTripsByDateRangeAsync(startDate, endDate);

        public async Task<IEnumerable<Trip>> SearchTripsAsync(string searchTerm)
        {
            var lowerTerm = searchTerm.ToLower();
            return await _tripRepository.FindAsync(t =>
                t.CustomerName.ToLower().Contains(lowerTerm) ||
                (t.CustomerPhone != null && t.CustomerPhone.Contains(searchTerm)));
        }

        public async Task CreateTripAsync(Trip trip)
        {
            await _tripRepository.AddAsync(trip);

            // If trip is created as InProgress, mark driver and vehicle as OnTrip
            if (trip.Status == TripStatus.InProgress || trip.Status == TripStatus.Confirmed)
            {
                await SetDriverVehicleOnTrip(trip.DriverId, trip.VehicleId);
            }

            await _tripRepository.SaveChangesAsync();
            _logger.LogInformation("Created Trip with ID {Id}", trip.Id);
        }

        public async Task UpdateTripAsync(Trip trip)
        {
            _tripRepository.Update(trip);
            await _tripRepository.SaveChangesAsync();
            _logger.LogInformation("Updated Trip with ID {Id}", trip.Id);
        }

        public async Task UpdateTripStatusAsync(int tripId, TripStatus newStatus)
        {
            var trip = await _tripRepository.GetByIdAsync(tripId);
            if (trip == null)
            {
                _logger.LogWarning("Trip with ID {Id} not found", tripId);
                throw new KeyNotFoundException($"Trip with ID {tripId} not found.");
            }

            var oldStatus = trip.Status;
            trip.Status = newStatus;
            _tripRepository.Update(trip);

            // Handle driver/vehicle status transitions
            if (newStatus == TripStatus.InProgress && oldStatus != TripStatus.InProgress)
            {
                await SetDriverVehicleOnTrip(trip.DriverId, trip.VehicleId);
            }
            else if ((newStatus == TripStatus.Completed || newStatus == TripStatus.Cancelled)
                     && oldStatus != TripStatus.Completed && oldStatus != TripStatus.Cancelled)
            {
                await ReleaseDriverVehicle(trip.DriverId, trip.VehicleId);
            }

            await _tripRepository.SaveChangesAsync();
            _logger.LogInformation("Updated Trip with ID {Id} status from {OldStatus} to {NewStatus}", tripId, oldStatus, newStatus);
        }

        public async Task DeleteTripAsync(int id)
        {
            var trip = await _tripRepository.GetByIdAsync(id);
            if (trip == null)
            {
                _logger.LogWarning("Trip with ID {Id} not found", id);
                throw new KeyNotFoundException($"Trip with ID {id} not found.");
            }

            // Release driver/vehicle if trip was active
            if (trip.Status == TripStatus.InProgress || trip.Status == TripStatus.Confirmed)
            {
                await ReleaseDriverVehicle(trip.DriverId, trip.VehicleId);
            }

            _tripRepository.Remove(trip);
            await _tripRepository.SaveChangesAsync();
            _logger.LogInformation("Deleted Trip with ID {Id}", id);
        }

        public async Task<int> GetTripCountAsync()
            => await _tripRepository.CountAsync();

        public async Task<int> GetActiveTripCountAsync()
            => await _tripRepository.CountAsync(t => t.Status == TripStatus.InProgress);

        private async Task SetDriverVehicleOnTrip(int driverId, int vehicleId)
        {
            var driver = await _driverRepository.GetByIdAsync(driverId);
            if (driver != null)
            {
                driver.Status = DriverStatus.OnTrip;
                _driverRepository.Update(driver);
            }

            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle != null)
            {
                vehicle.Status = VehicleStatus.OnTrip;
                _vehicleRepository.Update(vehicle);
            }
        }

        private async Task ReleaseDriverVehicle(int driverId, int vehicleId)
        {
            var driver = await _driverRepository.GetByIdAsync(driverId);
            if (driver != null)
            {
                driver.Status = DriverStatus.Available;
                _driverRepository.Update(driver);
            }

            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle != null)
            {
                vehicle.Status = VehicleStatus.Available;
                _vehicleRepository.Update(vehicle);
            }
        }
    }
}
