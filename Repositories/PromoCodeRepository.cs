using CabManagementSystem.Data;
using CabManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CabManagementSystem.Repositories;

public class PromoCodeRepository : Repository<PromoCode>, IPromoCodeRepository
{
    public PromoCodeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<PromoCode>> GetAllAsync()
        => await _dbSet.OrderByDescending(p => p.CreatedDate).ToListAsync();

    public async Task<PromoCode?> GetByCodeAsync(string code)
        => await _dbSet.FirstOrDefaultAsync(p => p.Code == code.ToUpper());

    public async Task<IEnumerable<PromoCode>> SearchAsync(string searchTerm)
        => await _dbSet.Where(p => p.Code.Contains(searchTerm) ||
                                   (p.Description != null && p.Description.Contains(searchTerm)))
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();
}