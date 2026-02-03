using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Repositories;
using Microsoft.Extensions.Logging;

namespace Cab_Management_System.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(IVehicleRepository vehicleRepository, ILogger<VehicleService> logger)
        {
            _vehicleRepository = vehicleRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            return await _vehicleRepository.GetAllAsync();
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(int id)
        {
            return await _vehicleRepository.GetByIdAsync(id);
        }

        public async Task<Vehicle?> GetVehicleWithMaintenanceAsync(int id)
        {
            return await _vehicleRepository.GetVehicleWithMaintenanceAsync(id);
        }

        public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
        {
            return await _vehicleRepository.GetAvailableVehiclesAsync();
        }

        public async Task<IEnumerable<Vehicle>> SearchVehiclesAsync(string searchTerm)
        {
            return await _vehicleRepository.SearchVehiclesAsync(searchTerm);
        }

        public async Task CreateVehicleAsync(Vehicle vehicle)
        {
            await _vehicleRepository.AddAsync(vehicle);
            await _vehicleRepository.SaveChangesAsync();
            _logger.LogInformation("Created Vehicle with ID {Id}", vehicle.Id);
        }

        public async Task UpdateVehicleAsync(Vehicle vehicle)
        {
            _vehicleRepository.Update(vehicle);
            await _vehicleRepository.SaveChangesAsync();
            _logger.LogInformation("Updated Vehicle with ID {Id}", vehicle.Id);
        }

        public async Task DeleteVehicleAsync(int id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
            {
                _logger.LogWarning("Vehicle with ID {Id} not found", id);
                throw new KeyNotFoundException($"Vehicle with ID {id} not found.");
            }

            _vehicleRepository.Remove(vehicle);
            await _vehicleRepository.SaveChangesAsync();
            _logger.LogInformation("Deleted Vehicle with ID {Id}", id);
        }

        public async Task<int> GetVehicleCountAsync()
        {
            return await _vehicleRepository.CountAsync();
        }

        public async Task<int> GetAvailableVehicleCountAsync()
        {
            return await _vehicleRepository.CountAsync(v => v.Status == VehicleStatus.Available);
        }
    }
}
