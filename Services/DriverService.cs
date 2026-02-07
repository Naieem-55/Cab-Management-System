using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Repositories;
using Microsoft.Extensions.Logging;

namespace Cab_Management_System.Services
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository _driverRepository;
        private readonly ITripRepository _tripRepository;
        private readonly ILogger<DriverService> _logger;

        public DriverService(IDriverRepository driverRepository, ITripRepository tripRepository, ILogger<DriverService> logger)
        {
            _driverRepository = driverRepository;
            _tripRepository = tripRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Driver>> GetAllDriversAsync()
        {
            return await _driverRepository.GetAllAsync();
        }

        public async Task<Driver?> GetDriverByIdAsync(int id)
        {
            return await _driverRepository.GetByIdAsync(id);
        }

        public async Task<Driver?> GetDriverWithEmployeeAsync(int id)
        {
            return await _driverRepository.GetDriverWithEmployeeAsync(id);
        }

        public async Task<IEnumerable<Driver>> GetAvailableDriversAsync()
        {
            return await _driverRepository.GetAvailableDriversAsync();
        }

        public async Task<IEnumerable<Driver>> SearchDriversAsync(string searchTerm)
        {
            return await _driverRepository.SearchDriversAsync(searchTerm);
        }

        public async Task CreateDriverAsync(Driver driver)
        {
            await _driverRepository.AddAsync(driver);
            await _driverRepository.SaveChangesAsync();
            _logger.LogInformation("Created Driver with ID {Id}", driver.Id);
        }

        public async Task UpdateDriverAsync(Driver driver)
        {
            _driverRepository.Update(driver);
            await _driverRepository.SaveChangesAsync();
            _logger.LogInformation("Updated Driver with ID {Id}", driver.Id);
        }

        public async Task DeleteDriverAsync(int id)
        {
            var driver = await _driverRepository.GetByIdAsync(id);
            if (driver == null)
            {
                _logger.LogWarning("Driver with ID {Id} not found", id);
                throw new KeyNotFoundException($"Driver with ID {id} not found.");
            }

            _driverRepository.Remove(driver);
            await _driverRepository.SaveChangesAsync();
            _logger.LogInformation("Deleted Driver with ID {Id}", id);
        }

        public async Task<int> GetDriverCountAsync()
        {
            return await _driverRepository.CountAsync();
        }

        public async Task<int> GetAvailableDriverCountAsync()
        {
            return await _driverRepository.CountAsync(d => d.Status == DriverStatus.Available);
        }

        public async Task<bool> CanDeleteAsync(int id)
        {
            var tripCount = await _tripRepository.CountAsync(t => t.DriverId == id);
            return tripCount == 0;
        }

        public async Task<int> GetTripCountAsync(int id)
        {
            return await _tripRepository.CountAsync(t => t.DriverId == id);
        }
    }
}
