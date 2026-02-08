using Cab_Management_System.Models;

namespace Cab_Management_System.Repositories
{
    public interface IDriverRepository : IRepository<Driver>
    {
        Task<IEnumerable<Driver>> GetAvailableDriversAsync();
        Task<IEnumerable<Driver>> SearchDriversAsync(string searchTerm);
        Task<Driver?> GetDriverWithEmployeeAsync(int id);
        Task<IEnumerable<Driver>> GetDriversWithExpiringLicensesAsync(int daysThreshold);
    }
}
