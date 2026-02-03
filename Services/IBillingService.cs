using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;

namespace Cab_Management_System.Services
{
    public interface IBillingService
    {
        Task<IEnumerable<Billing>> GetAllBillingsAsync();
        Task<Billing?> GetBillingByIdAsync(int id);
        Task<Billing?> GetBillingWithTripAsync(int id);
        Task<IEnumerable<Billing>> GetBillingsByStatusAsync(PaymentStatus status);
        Task<IEnumerable<Billing>> GetBillingsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task CreateBillingAsync(Billing billing);
        Task UpdateBillingAsync(Billing billing);
        Task DeleteBillingAsync(int id);
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetBillingCountAsync();
        Task<int> GetPendingBillingCountAsync();
    }
}
