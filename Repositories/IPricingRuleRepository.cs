using CabManagementSystem.Models;

namespace CabManagementSystem.Repositories;

public interface IPricingRuleRepository : IRepository<PricingRule>
{
    Task<IEnumerable<PricingRule>> GetActiveAsync();
    Task<IEnumerable<PricingRule>> SearchAsync(string searchTerm);
}
