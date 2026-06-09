using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;

namespace CabManagementSystem.Repositories
{
    public interface ILoyaltyTransactionRepository : IRepository<LoyaltyTransaction>
    {
        Task<IEnumerable<LoyaltyTransaction>> GetByCustomerIdAsync(int customerId);
        Task<bool> HasTransactionForTripAsync(int tripId, LoyaltyTransactionType type);
    }
}
