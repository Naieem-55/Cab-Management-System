using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;

namespace CabManagementSystem.Repositories
{
    public interface IBillingRepository : IRepository<Billing>
    {
        Task<Billing?> GetBillingWithTripAsync(int id);
        Task<IEnumerable<Billing>> GetBillingsByStatusAsync(PaymentStatus status);
        Task<IEnumerable<Billing>> GetBillingsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalRevenueAsync();
        Task<IEnumerable<Billing>> GetBillingsByCustomerIdAsync(int customerId);
    }
}
