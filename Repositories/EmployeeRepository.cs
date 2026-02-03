using Cab_Management_System.Data;
using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Employee>> GetAllAsync()
            => await _dbSet.OrderBy(e => e.Name).ToListAsync();

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionAsync(EmployeePosition position)
            => await _dbSet.Where(e => e.Position == position)
                           .OrderBy(e => e.Name)
                           .ToListAsync();

        public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm)
            => await _dbSet.Where(e => e.Name.Contains(searchTerm) ||
                                       e.Email.Contains(searchTerm) ||
                                       e.Phone.Contains(searchTerm))
                           .OrderBy(e => e.Name)
                           .ToListAsync();

        public async Task<Employee?> GetEmployeeWithDriverAsync(int id)
            => await _dbSet.Include(e => e.Driver)
                           .FirstOrDefaultAsync(e => e.Id == id);
    }
}
