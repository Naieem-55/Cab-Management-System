using CabManagementSystem.Models;

namespace CabManagementSystem.Services
{
    public interface IPromoCodeService
    {
        Task<IEnumerable<PromoCode>> GetAllPromoCodesAsync();
        Task<PromoCode?> GetPromoCodeByIdAsync(int id);
        Task<IEnumerable<PromoCode>> SearchPromoCodesAsync(string searchTerm);
        Task CreatePromoCodeAsync(PromoCode promoCode);
        Task UpdatePromoCodeAsync(PromoCode promoCode);
        Task DeletePromoCodeAsync(int id);
        Task<int> GetPromoCodeCountAsync();
        Task<PromoValidationResult> ValidateAsync(string code, decimal tripCost);
        Task ApplyUsageAsync(int promoCodeId);
    }
}
