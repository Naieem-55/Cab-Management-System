using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Repositories;
using Microsoft.Extensions.Logging;

namespace Cab_Management_System.Services
{
    public class BillingService : IBillingService
    {
        private readonly IBillingRepository _billingRepository;
        private readonly ILogger<BillingService> _logger;

        public BillingService(IBillingRepository billingRepository, ILogger<BillingService> logger)
        {
            _billingRepository = billingRepository;
            _logger = logger;
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
            _logger.LogInformation("Created Billing with ID {Id}", billing.Id);
        }

        public async Task UpdateBillingAsync(Billing billing)
        {
            _billingRepository.Update(billing);
            await _billingRepository.SaveChangesAsync();
            _logger.LogInformation("Updated Billing with ID {Id}", billing.Id);
        }

        public async Task DeleteBillingAsync(int id)
        {
            var billing = await _billingRepository.GetByIdAsync(id);
            if (billing == null)
            {
                _logger.LogWarning("Billing with ID {Id} not found", id);
                throw new KeyNotFoundException($"Billing with ID {id} not found.");
            }

            _billingRepository.Remove(billing);
            await _billingRepository.SaveChangesAsync();
            _logger.LogInformation("Deleted Billing with ID {Id}", id);
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

        public async Task<IEnumerable<Billing>> GetBillingsByCustomerIdAsync(int customerId)
        {
            return await _billingRepository.GetBillingsByCustomerIdAsync(customerId);
        }
    }
}
