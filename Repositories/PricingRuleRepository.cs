using CabManagementSystem.Data;
using CabManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CabManagementSystem.Repositories;

public class PricingRuleRepository : Repository<PricingRule>, IPricingRuleRepository
{
    public PricingRuleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<PricingRule>> GetAllAsync()
        => await _dbSet.OrderBy(r => r.StartTime).ThenBy(r => r.Name).ToListAsync();

    public async Task<IEnumerable<PricingRule>> GetActiveAsync()
        => await _dbSet.AsNoTracking().Where(r => r.IsActive).ToListAsync();

    public async Task<IEnumerable<PricingRule>> SearchAsync(string searchTerm)
        => await _dbSet.Where(r => r.Name.Contains(searchTerm) ||
                                   (r.Description != null && r.Description.Contains(searchTerm)))
            .OrderBy(r => r.StartTime)
            .ToListAsync();
}
