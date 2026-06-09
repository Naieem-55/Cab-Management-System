using CabManagementSystem.Data;
using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CabManagementSystem.Repositories
{
    public class LoyaltyTransactionRepository : Repository<LoyaltyTransaction>, ILoyaltyTransactionRepository
    {
        public LoyaltyTransactionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<LoyaltyTransaction>> GetAllAsync()
        {
            return await _context.LoyaltyTransactions
                .Include(lt => lt.Trip).ThenInclude(t => t!.Route)
                .OrderByDescending(lt => lt.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<LoyaltyTransaction>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.LoyaltyTransactions
                .Include(lt => lt.Trip).ThenInclude(t => t!.Route)
                .Where(lt => lt.CustomerId == customerId)
                .OrderByDescending(lt => lt.CreatedDate)
                .ToListAsync();
        }

        public async Task<bool> HasTransactionForTripAsync(int tripId, LoyaltyTransactionType type)
        {
            return await _context.LoyaltyTransactions
                .AnyAsync(lt => lt.TripId == tripId && lt.Type == type);
        }
    }
}
