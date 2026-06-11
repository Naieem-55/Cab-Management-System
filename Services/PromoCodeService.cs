using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;
using CabManagementSystem.Repositories;

namespace CabManagementSystem.Services
{
    public class PromoCodeService : IPromoCodeService
    {
        private readonly IPromoCodeRepository _promoCodeRepository;
        private readonly ILogger<PromoCodeService> _logger;

        public PromoCodeService(IPromoCodeRepository promoCodeRepository, ILogger<PromoCodeService> logger)
        {
            _promoCodeRepository = promoCodeRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<PromoCode>> GetAllPromoCodesAsync()
            => await _promoCodeRepository.GetAllAsync();

        public async Task<PromoCode?> GetPromoCodeByIdAsync(int id)
            => await _promoCodeRepository.GetByIdAsync(id);

        public async Task<IEnumerable<PromoCode>> SearchPromoCodesAsync(string searchTerm)
            => await _promoCodeRepository.SearchAsync(searchTerm);

        public async Task CreatePromoCodeAsync(PromoCode promoCode)
        {
            promoCode.Code = promoCode.Code.Trim().ToUpper();
            await _promoCodeRepository.AddAsync(promoCode);
            await _promoCodeRepository.SaveChangesAsync();
            _logger.LogInformation("Created PromoCode {Code}", promoCode.Code);
        }

        public async Task UpdatePromoCodeAsync(PromoCode promoCode)
        {
            promoCode.Code = promoCode.Code.Trim().ToUpper();
            _promoCodeRepository.Update(promoCode);
            await _promoCodeRepository.SaveChangesAsync();
            _logger.LogInformation("Updated PromoCode {Id}", promoCode.Id);
        }

        public async Task DeletePromoCodeAsync(int id)
        {
            var promoCode = await _promoCodeRepository.GetByIdAsync(id);
            if (promoCode == null)
                throw new KeyNotFoundException($"PromoCode with ID {id} not found.");

            _promoCodeRepository.Remove(promoCode);
            await _promoCodeRepository.SaveChangesAsync();
            _logger.LogInformation("Deleted PromoCode {Id}", id);
        }

        public async Task<int> GetPromoCodeCountAsync()
            => await _promoCodeRepository.CountAsync();

        public async Task<PromoValidationResult> ValidateAsync(string code, decimal tripCost)
        {
            if (string.IsNullOrWhiteSpace(code))
                return PromoValidationResult.Fail("No promo code entered.");

            var promo = await _promoCodeRepository.GetByCodeAsync(code.Trim());
            if (promo == null)
                return PromoValidationResult.Fail("Promo code not found.");

            if (!promo.IsActive)
                return PromoValidationResult.Fail("This promo code is inactive.");

            var now = DateTime.Now;
            if (now < promo.ValidFrom)
                return PromoValidationResult.Fail("This promo code is not yet active.");
            if (now > promo.ValidUntil)
                return PromoValidationResult.Fail("This promo code has expired.");

            if (promo.UsageLimit.HasValue && promo.TimesUsed >= promo.UsageLimit.Value)
                return PromoValidationResult.Fail("This promo code has reached its usage limit.");

            if (tripCost < promo.MinTripCost)
                return PromoValidationResult.Fail($"Trip cost must be at least {promo.MinTripCost:C} to use this code.");

            decimal discount = promo.DiscountType == DiscountType.Percentage
                ? tripCost * (promo.DiscountValue / 100m)
                : promo.DiscountValue;

            if (promo.MaxDiscountAmount.HasValue && discount > promo.MaxDiscountAmount.Value)
                discount = promo.MaxDiscountAmount.Value;

            if (discount > tripCost)
                discount = tripCost;

            return new PromoValidationResult
            {
                IsValid = true,
                Message = $"{promo.Code} applied — {discount:C} off.",
                DiscountAmount = discount,
                PromoCodeId = promo.Id,
                Code = promo.Code
            };
        }

        public async Task ApplyUsageAsync(int promoCodeId)
        {
            var promo = await _promoCodeRepository.GetByIdAsync(promoCodeId);
            if (promo == null)
                return;

            promo.TimesUsed += 1;
            _promoCodeRepository.Update(promo);
            await _promoCodeRepository.SaveChangesAsync();
        }
    }
}
