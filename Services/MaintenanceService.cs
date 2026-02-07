using Cab_Management_System.Models;
using Cab_Management_System.Repositories;
using Microsoft.Extensions.Logging;

namespace Cab_Management_System.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IMaintenanceRepository _maintenanceRepository;
        private readonly ILogger<MaintenanceService> _logger;

        public MaintenanceService(IMaintenanceRepository maintenanceRepository, ILogger<MaintenanceService> logger)
        {
            _maintenanceRepository = maintenanceRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<MaintenanceRecord>> GetAllMaintenanceRecordsAsync()
        {
            return await _maintenanceRepository.GetAllAsync();
        }

        public async Task<MaintenanceRecord?> GetMaintenanceByIdAsync(int id)
        {
            return await _maintenanceRepository.GetByIdAsync(id);
        }

        public async Task<MaintenanceRecord?> GetMaintenanceWithVehicleAsync(int id)
        {
            return await _maintenanceRepository.GetMaintenanceWithVehicleAsync(id);
        }

        public async Task<IEnumerable<MaintenanceRecord>> GetOverdueMaintenanceAsync()
        {
            return await _maintenanceRepository.GetOverdueMaintenanceAsync();
        }

        public async Task<IEnumerable<MaintenanceRecord>> GetMaintenanceByVehicleAsync(int vehicleId)
        {
            return await _maintenanceRepository.GetMaintenanceByVehicleAsync(vehicleId);
        }

        public async Task CreateMaintenanceAsync(MaintenanceRecord maintenanceRecord)
        {
            await _maintenanceRepository.AddAsync(maintenanceRecord);
            await _maintenanceRepository.SaveChangesAsync();
            _logger.LogInformation("Created MaintenanceRecord with ID {Id}", maintenanceRecord.Id);
        }

        public async Task UpdateMaintenanceAsync(MaintenanceRecord maintenanceRecord)
        {
            _maintenanceRepository.Update(maintenanceRecord);
            await _maintenanceRepository.SaveChangesAsync();
            _logger.LogInformation("Updated MaintenanceRecord with ID {Id}", maintenanceRecord.Id);
        }

        public async Task DeleteMaintenanceAsync(int id)
        {
            var maintenanceRecord = await _maintenanceRepository.GetByIdAsync(id);
            if (maintenanceRecord == null)
            {
                _logger.LogWarning("MaintenanceRecord with ID {Id} not found", id);
                throw new KeyNotFoundException($"Maintenance record with ID {id} not found.");
            }

            _maintenanceRepository.Remove(maintenanceRecord);
            await _maintenanceRepository.SaveChangesAsync();
            _logger.LogInformation("Deleted MaintenanceRecord with ID {Id}", id);
        }

        public async Task<int> GetMaintenanceCountAsync()
        {
            return await _maintenanceRepository.CountAsync();
        }

        public async Task<IEnumerable<MaintenanceRecord>> SearchMaintenanceAsync(string searchTerm)
        {
            return await _maintenanceRepository.SearchMaintenanceAsync(searchTerm);
        }
    }
}
