using CabManagementSystem.Models;

namespace CabManagementSystem.Services
{
    public interface IPricingService
    {
        Task<IEnumerable<PricingRule>> GetAllRulesAsync();
        Task<PricingRule?> GetRuleByIdAsync(int id);
        Task<IEnumerable<PricingRule>> SearchRulesAsync(string searchTerm);
        Task CreateRuleAsync(PricingRule rule);
        Task UpdateRuleAsync(PricingRule rule);
        Task DeleteRuleAsync(int id);

        /// <summary>Base fare plus the single highest matching surcharge for the trip time.</summary>
        Task<FareQuote> GetQuoteAsync(decimal baseFare, DateTime tripDate);
    }
}
