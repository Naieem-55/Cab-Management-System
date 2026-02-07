using Cab_Management_System.Models;

namespace Cab_Management_System.Services
{
    public interface IVehicleService
    {
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle?> GetVehicleByIdAsync(int id);
        Task<Vehicle?> GetVehicleWithMaintenanceAsync(int id);
        Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();
        Task<IEnumerable<Vehicle>> SearchVehiclesAsync(string searchTerm);
        Task CreateVehicleAsync(Vehicle vehicle);
        Task UpdateVehicleAsync(Vehicle vehicle);
        Task DeleteVehicleAsync(int id);
        Task<int> GetVehicleCountAsync();
        Task<int> GetAvailableVehicleCountAsync();
        Task<bool> CanDeleteAsync(int id);
        Task<(int TripCount, int MaintenanceCount)> GetDependencyCountsAsync(int id);
    }
}
