using CabManagementSystem.Models;

namespace CabManagementSystem.Repositories
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();
        Task<IEnumerable<Vehicle>> SearchVehiclesAsync(string searchTerm);
        Task<Vehicle?> GetVehicleWithMaintenanceAsync(int id);
    }
}
