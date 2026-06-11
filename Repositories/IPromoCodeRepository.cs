using CabManagementSystem.Models;

namespace CabManagementSystem.Repositories;

public interface IPromoCodeRepository : IRepository<PromoCode>
{
    Task<PromoCode?> GetByCodeAsync(string code);
    Task<IEnumerable<PromoCode>> SearchAsync(string searchTerm);
}