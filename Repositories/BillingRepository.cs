using Cab_Management_System.Data;
using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Repositories
{
    public class BillingRepository : Repository<Billing>, IBillingRepository
    {
        public BillingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Billing>> GetAllAsync()
            => await _dbSet.Include(b => b.Trip)
                           .OrderByDescending(b => b.PaymentDate)
                           .ToListAsync();

        public async Task<Billing?> GetBillingWithTripAsync(int id)
            => await _dbSet.Include(b => b.Trip)
                               .ThenInclude(t => t.Driver)
                                   .ThenInclude(d => d.Employee)
                           .Include(b => b.Trip)
                               .ThenInclude(t => t.Vehicle)
                           .Include(b => b.Trip)
                               .ThenInclude(t => t.Route)
                           .FirstOrDefaultAsync(b => b.Id == id);

        public async Task<IEnumerable<Billing>> GetBillingsByStatusAsync(PaymentStatus status)
            => await _dbSet.Include(b => b.Trip)
                           .Where(b => b.Status == status)
                           .OrderByDescending(b => b.PaymentDate)
                           .ToListAsync();

        public async Task<IEnumerable<Billing>> GetBillingsByDateRangeAsync(DateTime startDate, DateTime endDate)
            => await _dbSet.Include(b => b.Trip)
                           .Where(b => b.PaymentDate >= startDate && b.PaymentDate <= endDate)
                           .OrderByDescending(b => b.PaymentDate)
                           .ToListAsync();

        public async Task<decimal> GetTotalRevenueAsync()
            => await _dbSet.Where(b => b.Status == PaymentStatus.Completed)
                           .SumAsync(b => b.Amount);
    }
}
