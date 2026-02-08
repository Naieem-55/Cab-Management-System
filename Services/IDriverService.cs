using Cab_Management_System.Models;

namespace Cab_Management_System.Services
{
    public interface IDriverService
    {
        Task<IEnumerable<Driver>> GetAllDriversAsync();
        Task<Driver?> GetDriverByIdAsync(int id);
        Task<Driver?> GetDriverWithEmployeeAsync(int id);
        Task<IEnumerable<Driver>> GetAvailableDriversAsync();
        Task<IEnumerable<Driver>> SearchDriversAsync(string searchTerm);
        Task CreateDriverAsync(Driver driver);
        Task UpdateDriverAsync(Driver driver);
        Task DeleteDriverAsync(int id);
        Task<int> GetDriverCountAsync();
        Task<int> GetAvailableDriverCountAsync();
        Task<bool> CanDeleteAsync(int id);
        Task<int> GetTripCountAsync(int id);
        Task<IEnumerable<Driver>> GetDriversWithExpiringLicensesAsync(int daysThreshold = 30);
    }
}
