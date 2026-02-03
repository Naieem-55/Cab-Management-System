using Cab_Management_System.Models;

namespace Cab_Management_System.Repositories
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();
        Task<IEnumerable<Vehicle>> SearchVehiclesAsync(string searchTerm);
        Task<Vehicle?> GetVehicleWithMaintenanceAsync(int id);
    }
}
