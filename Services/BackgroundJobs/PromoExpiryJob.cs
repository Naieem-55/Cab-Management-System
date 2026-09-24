using CabManagementSystem.Models.Enums;
using CabManagementSystem.Repositories;
using Microsoft.Extensions.Options;

namespace CabManagementSystem.Services.BackgroundJobs
{
    /// <summary>
    /// Deactivates promo codes that have passed their validity window or used up their
    /// allowance, so expired offers stop appearing as active.
    /// </summary>
    public class PromoExpiryJob : IScheduledJob
    {
        private readonly IPromoCodeRepository _promoCodeRepository;
        private readonly JobNotifier _notifier;
        private readonly BackgroundJobOptions _options;

        public PromoExpiryJob(
            IPromoCodeRepository promoCodeRepository,
            JobNotifier notifier,
            IOptions<BackgroundJobOptions> options)
        {
            _promoCodeRepository = promoCodeRepository;
            _notifier = notifier;
            _options = options.Value;
        }

        public string Name => "PromoExpiry";

        public TimeSpan Interval => _options.PromoExpiryInterval;

        public async Task<string> RunAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.Now;

            var spent = (await _promoCodeRepository.FindAsync(p =>
                p.IsActive &&
                (p.ValidUntil < now ||
                 (p.UsageLimit != null && p.TimesUsed >= p.UsageLimit)))).ToList();

            foreach (var promo in spent)
            {
                cancellationToken.ThrowIfCancellationRequested();

                promo.IsActive = false;
                _promoCodeRepository.Update(promo);
            }

            if (spent.Count > 0)
            {
                await _promoCodeRepository.SaveChangesAsync();

                var codes = string.Join(", ", spent.Select(p => p.Code));
                await _notifier.NotifyRolesAsync(
                    $"{spent.Count} promo code(s) deactivated",
                    $"Expired or fully used promo codes were switched off: {codes}",
                    "/Admin/PromoCode",
                    UserRole.Admin);
            }

            return $"{spent.Count} promo code(s) deactivated";
        }
    }
}
