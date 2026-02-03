using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;

namespace Cab_Management_System.Repositories
{
    public interface IBillingRepository : IRepository<Billing>
    {
        Task<Billing?> GetBillingWithTripAsync(int id);
        Task<IEnumerable<Billing>> GetBillingsByStatusAsync(PaymentStatus status);
        Task<IEnumerable<Billing>> GetBillingsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalRevenueAsync();
    }
}
