using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Repositories;

namespace Cab_Management_System.Services
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository _driverRepository;

        public DriverService(IDriverRepository driverRepository)
        {
            _driverRepository = driverRepository;
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
        }

        public async Task UpdateDriverAsync(Driver driver)
        {
            _driverRepository.Update(driver);
            await _driverRepository.SaveChangesAsync();
        }

        public async Task DeleteDriverAsync(int id)
        {
            var driver = await _driverRepository.GetByIdAsync(id);
            if (driver == null)
                throw new KeyNotFoundException($"Driver with ID {id} not found.");

            _driverRepository.Remove(driver);
            await _driverRepository.SaveChangesAsync();
        }

        public async Task<int> GetDriverCountAsync()
        {
            return await _driverRepository.CountAsync();
        }

        public async Task<int> GetAvailableDriverCountAsync()
        {
            return await _driverRepository.CountAsync(d => d.Status == DriverStatus.Available);
        }
    }
}
