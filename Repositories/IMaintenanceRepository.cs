using CabManagementSystem.Models;

namespace CabManagementSystem.Repositories
{
    public interface IMaintenanceRepository : IRepository<MaintenanceRecord>
    {
        Task<MaintenanceRecord?> GetMaintenanceWithVehicleAsync(int id);
        Task<IEnumerable<MaintenanceRecord>> GetOverdueMaintenanceAsync();

        /// <summary>Unfinished records whose next service date falls on or before the given moment, vehicle included.</summary>
        Task<IEnumerable<MaintenanceRecord>> GetDueMaintenanceAsync(DateTime horizon);
        Task<IEnumerable<MaintenanceRecord>> GetMaintenanceByVehicleAsync(int vehicleId);
        Task<IEnumerable<MaintenanceRecord>> SearchMaintenanceAsync(string searchTerm);
    }
}
