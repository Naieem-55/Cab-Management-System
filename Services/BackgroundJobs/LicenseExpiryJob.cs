using CabManagementSystem.Models.Enums;
using Microsoft.Extensions.Options;

namespace CabManagementSystem.Services.BackgroundJobs
{
    /// <summary>Warns HR and admins about driving licences that expired or are about to.</summary>
    public class LicenseExpiryJob : IScheduledJob
    {
        private readonly IDriverService _driverService;
        private readonly JobNotifier _notifier;
        private readonly BackgroundJobOptions _options;

        public LicenseExpiryJob(IDriverService driverService, JobNotifier notifier, IOptions<BackgroundJobOptions> options)
        {
            _driverService = driverService;
            _notifier = notifier;
            _options = options.Value;
        }

        public string Name => "LicenseExpiry";

        public TimeSpan Interval => _options.LicenseExpiryInterval;

        public async Task<string> RunAsync(CancellationToken cancellationToken)
        {
            var drivers = (await _driverService.GetDriversWithExpiringLicensesAsync(_options.LicenseExpiryWarningDays)).ToList();
            var today = DateTime.Today;
            var notified = 0;

            foreach (var driver in drivers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var name = driver.Employee?.Name ?? $"Driver #{driver.Id}";
                var daysLeft = (driver.LicenseExpiry.Date - today).Days;

                var title = daysLeft < 0
                    ? $"Licence expired: {name}"
                    : $"Licence expiring: {name}";

                var message = daysLeft < 0
                    ? $"Licence {driver.LicenseNumber} expired on {driver.LicenseExpiry:dd MMM yyyy} ({-daysLeft} day(s) ago). The driver should not be assigned trips."
                    : $"Licence {driver.LicenseNumber} expires on {driver.LicenseExpiry:dd MMM yyyy} ({daysLeft} day(s) left).";

                notified += await _notifier.NotifyRolesAsync(
                    title, message, $"/HR/Driver/Details/{driver.Id}",
                    UserRole.HRManager, UserRole.Admin);
            }

            return $"{drivers.Count} licence(s) within {_options.LicenseExpiryWarningDays} days, {notified} notification(s) sent";
        }
    }
}
