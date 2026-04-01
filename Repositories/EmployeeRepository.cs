using CabManagementSystem.Data;
using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CabManagementSystem.Repositories
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
