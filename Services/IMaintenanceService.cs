using Cab_Management_System.Models;

namespace Cab_Management_System.Services
{
    public interface IMaintenanceService
    {
        Task<IEnumerable<MaintenanceRecord>> GetAllMaintenanceRecordsAsync();
        Task<MaintenanceRecord?> GetMaintenanceByIdAsync(int id);
        Task<MaintenanceRecord?> GetMaintenanceWithVehicleAsync(int id);
        Task<IEnumerable<MaintenanceRecord>> GetOverdueMaintenanceAsync();
        Task<IEnumerable<MaintenanceRecord>> GetMaintenanceByVehicleAsync(int vehicleId);
        Task CreateMaintenanceAsync(MaintenanceRecord maintenanceRecord);
        Task UpdateMaintenanceAsync(MaintenanceRecord maintenanceRecord);
        Task DeleteMaintenanceAsync(int id);
        Task<int> GetMaintenanceCountAsync();
        Task<IEnumerable<MaintenanceRecord>> SearchMaintenanceAsync(string searchTerm);
    }
}
