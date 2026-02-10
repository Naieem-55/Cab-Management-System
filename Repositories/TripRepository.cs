using Cab_Management_System.Data;
using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Repositories
{
    public class TripRepository : Repository<Trip>, ITripRepository
    {
        public TripRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Trip>> GetAllAsync()
            => await _dbSet.Include(t => t.Driver)
                               .ThenInclude(d => d.Employee)
                           .Include(t => t.Vehicle)
                           .Include(t => t.Route)
                           .OrderByDescending(t => t.BookingDate)
                           .ToListAsync();

        public async Task<Trip?> GetTripWithDetailsAsync(int id)
            => await _dbSet.Include(t => t.Driver)
                               .ThenInclude(d => d.Employee)
                           .Include(t => t.Vehicle)
                           .Include(t => t.Route)
                           .Include(t => t.Billing)
                           .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<IEnumerable<Trip>> GetTripsByStatusAsync(TripStatus status)
            => await _dbSet.Include(t => t.Driver)
                               .ThenInclude(d => d.Employee)
                           .Include(t => t.Vehicle)
                           .Include(t => t.Route)
                           .Where(t => t.Status == status)
                           .OrderByDescending(t => t.BookingDate)
                           .ToListAsync();

        public async Task<IEnumerable<Trip>> GetTripsByDateRangeAsync(DateTime startDate, DateTime endDate)
            => await _dbSet.Include(t => t.Driver)
                               .ThenInclude(d => d.Employee)
                           .Include(t => t.Vehicle)
                           .Include(t => t.Route)
                           .Where(t => t.TripDate >= startDate && t.TripDate <= endDate)
                           .OrderByDescending(t => t.BookingDate)
                           .ToListAsync();

        public async Task<IEnumerable<Trip>> GetTripsByCustomerIdAsync(int customerId)
            => await _dbSet.Include(t => t.Driver)
                               .ThenInclude(d => d.Employee)
                           .Include(t => t.Vehicle)
                           .Include(t => t.Route)
                           .Include(t => t.Billing)
                           .Where(t => t.CustomerId == customerId)
                           .OrderByDescending(t => t.BookingDate)
                           .ToListAsync();
    }
}
