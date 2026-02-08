using Cab_Management_System.Data;
using Cab_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Customer>> GetAllAsync()
            => await _dbSet.Include(c => c.Trips)
                           .OrderBy(c => c.Name)
                           .ToListAsync();

        public async Task<Customer?> GetCustomerWithTripsAsync(int id)
            => await _dbSet.Include(c => c.Trips)
                               .ThenInclude(t => t.Route)
                           .Include(c => c.Trips)
                               .ThenInclude(t => t.Driver)
                                   .ThenInclude(d => d.Employee)
                           .Include(c => c.Trips)
                               .ThenInclude(t => t.Vehicle)
                           .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
            => await _dbSet.Include(c => c.Trips)
                           .Where(c => c.Name.Contains(searchTerm) ||
                                       c.Email.Contains(searchTerm) ||
                                       c.Phone.Contains(searchTerm))
                           .OrderBy(c => c.Name)
                           .ToListAsync();
    }
}
