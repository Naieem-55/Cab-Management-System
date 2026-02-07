using Cab_Management_System.Models;

namespace Cab_Management_System.Repositories
{
    public interface IMaintenanceRepository : IRepository<MaintenanceRecord>
    {
        Task<MaintenanceRecord?> GetMaintenanceWithVehicleAsync(int id);
        Task<IEnumerable<MaintenanceRecord>> GetOverdueMaintenanceAsync();
        Task<IEnumerable<MaintenanceRecord>> GetMaintenanceByVehicleAsync(int vehicleId);
        Task<IEnumerable<MaintenanceRecord>> SearchMaintenanceAsync(string searchTerm);
    }
}
