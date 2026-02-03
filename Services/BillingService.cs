using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Repositories;

namespace Cab_Management_System.Services
{
    public class BillingService : IBillingService
    {
        private readonly IBillingRepository _billingRepository;

        public BillingService(IBillingRepository billingRepository)
        {
            _billingRepository = billingRepository;
        }

        public async Task<IEnumerable<Billing>> GetAllBillingsAsync()
        {
            return await _billingRepository.GetAllAsync();
        }

        public async Task<Billing?> GetBillingByIdAsync(int id)
        {
            return await _billingRepository.GetByIdAsync(id);
        }

        public async Task<Billing?> GetBillingWithTripAsync(int id)
        {
            return await _billingRepository.GetBillingWithTripAsync(id);
        }

        public async Task<IEnumerable<Billing>> GetBillingsByStatusAsync(PaymentStatus status)
        {
            return await _billingRepository.GetBillingsByStatusAsync(status);
        }

        public async Task<IEnumerable<Billing>> GetBillingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _billingRepository.GetBillingsByDateRangeAsync(startDate, endDate);
        }

        public async Task CreateBillingAsync(Billing billing)
        {
            await _billingRepository.AddAsync(billing);
            await _billingRepository.SaveChangesAsync();
        }

        public async Task UpdateBillingAsync(Billing billing)
        {
            _billingRepository.Update(billing);
            await _billingRepository.SaveChangesAsync();
        }

        public async Task DeleteBillingAsync(int id)
        {
            var billing = await _billingRepository.GetByIdAsync(id);
            if (billing == null)
                throw new KeyNotFoundException($"Billing with ID {id} not found.");

            _billingRepository.Remove(billing);
            await _billingRepository.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _billingRepository.GetTotalRevenueAsync();
        }

        public async Task<int> GetBillingCountAsync()
        {
            return await _billingRepository.CountAsync();
        }

        public async Task<int> GetPendingBillingCountAsync()
        {
            return await _billingRepository.CountAsync(b => b.Status == PaymentStatus.Pending);
        }
    }
}
