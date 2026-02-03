using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Repositories;

namespace Cab_Management_System.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleService(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
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
        }

        public async Task UpdateVehicleAsync(Vehicle vehicle)
        {
            _vehicleRepository.Update(vehicle);
            await _vehicleRepository.SaveChangesAsync();
        }

        public async Task DeleteVehicleAsync(int id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
                throw new KeyNotFoundException($"Vehicle with ID {id} not found.");

            _vehicleRepository.Remove(vehicle);
            await _vehicleRepository.SaveChangesAsync();
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
