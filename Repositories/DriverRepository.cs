using Cab_Management_System.Data;
using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Repositories
{
    public class DriverRepository : Repository<Driver>, IDriverRepository
    {
        public DriverRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Driver>> GetAllAsync()
            => await _dbSet.Include(d => d.Employee)
                           .OrderBy(d => d.Employee.Name)
                           .ToListAsync();

        public async Task<IEnumerable<Driver>> GetAvailableDriversAsync()
            => await _dbSet.Include(d => d.Employee)
                           .Where(d => d.Status == DriverStatus.Available)
                           .OrderBy(d => d.Employee.Name)
                           .ToListAsync();

        public async Task<IEnumerable<Driver>> SearchDriversAsync(string searchTerm)
            => await _dbSet.Include(d => d.Employee)
                           .Where(d => d.Employee.Name.Contains(searchTerm) ||
                                       d.LicenseNumber.Contains(searchTerm) ||
                                       d.Employee.Email.Contains(searchTerm))
                           .OrderBy(d => d.Employee.Name)
                           .ToListAsync();

        public async Task<Driver?> GetDriverWithEmployeeAsync(int id)
            => await _dbSet.Include(d => d.Employee)
                           .FirstOrDefaultAsync(d => d.Id == id);
    }
}
