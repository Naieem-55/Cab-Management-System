using CabManagementSystem.Models;
using CabManagementSystem.Repositories;

namespace CabManagementSystem.Services
{
    public class PricingService : IPricingService
    {
        private readonly IPricingRuleRepository _ruleRepository;
        private readonly ILogger<PricingService> _logger;

        public PricingService(IPricingRuleRepository ruleRepository, ILogger<PricingService> logger)
        {
            _ruleRepository = ruleRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<PricingRule>> GetAllRulesAsync()
            => await _ruleRepository.GetAllAsync();

        public async Task<PricingRule?> GetRuleByIdAsync(int id)
            => await _ruleRepository.GetByIdAsync(id);

        public async Task<IEnumerable<PricingRule>> SearchRulesAsync(string searchTerm)
            => await _ruleRepository.SearchAsync(searchTerm);

        public async Task CreateRuleAsync(PricingRule rule)
        {
            await _ruleRepository.AddAsync(rule);
            await _ruleRepository.SaveChangesAsync();
            _logger.LogInformation("Created PricingRule {Name}", rule.Name);
        }

        public async Task UpdateRuleAsync(PricingRule rule)
        {
            _ruleRepository.Update(rule);
            await _ruleRepository.SaveChangesAsync();
            _logger.LogInformation("Updated PricingRule {Id}", rule.Id);
        }

        public async Task DeleteRuleAsync(int id)
        {
            var rule = await _ruleRepository.GetByIdAsync(id);
            if (rule == null)
                throw new KeyNotFoundException($"PricingRule with ID {id} not found.");

            _ruleRepository.Remove(rule);
            await _ruleRepository.SaveChangesAsync();
            _logger.LogInformation("Deleted PricingRule {Id}", id);
        }

        public async Task<FareQuote> GetQuoteAsync(decimal baseFare, DateTime tripDate)
            => BuildQuote(baseFare, tripDate, await _ruleRepository.GetActiveAsync());

        // Overlapping rules do not stack: the one yielding the largest surcharge wins.
        public static FareQuote BuildQuote(decimal baseFare, DateTime tripDate, IEnumerable<PricingRule> rules)
        {
            var best = rules
                .Where(r => r.AppliesAt(tripDate))
                .Select(r => new { Rule = r, Amount = r.CalculateSurcharge(baseFare) })
                .OrderByDescending(x => x.Amount)
                .FirstOrDefault();

            if (best == null || best.Amount <= 0)
                return new FareQuote { BaseFare = baseFare };

            return new FareQuote
            {
                BaseFare = baseFare,
                Surcharge = best.Amount,
                PricingRuleId = best.Rule.Id,
                SurchargeLabel = best.Rule.Name
            };
        }
    }
}
