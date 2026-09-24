using CabManagementSystem.Models.Enums;
using Microsoft.Extensions.Options;

namespace CabManagementSystem.Services.BackgroundJobs
{
    /// <summary>
    /// Cancels trips still sitting in Pending well after their trip time, so they stop
    /// holding a driver and vehicle and stop showing up as upcoming work.
    /// </summary>
    public class StaleTripCleanupJob : IScheduledJob
    {
        private readonly ITripService _tripService;
        private readonly JobNotifier _notifier;
        private readonly BackgroundJobOptions _options;
        private readonly ILogger<StaleTripCleanupJob> _logger;

        public StaleTripCleanupJob(
            ITripService tripService,
            JobNotifier notifier,
            IOptions<BackgroundJobOptions> options,
            ILogger<StaleTripCleanupJob> logger)
        {
            _tripService = tripService;
            _notifier = notifier;
            _options = options.Value;
            _logger = logger;
        }

        public string Name => "StaleTripCleanup";

        public TimeSpan Interval => _options.StaleTripInterval;

        public async Task<string> RunAsync(CancellationToken cancellationToken)
        {
            var cutoff = DateTime.Now.AddHours(-_options.StaleTripGraceHours);

            var stale = (await _tripService.GetTripsByStatusAsync(TripStatus.Pending))
                .Where(t => t.TripDate < cutoff)
                .ToList();

            var cancelled = 0;

            foreach (var trip in stale)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    // Goes through the service so the status history and driver/vehicle release run too.
                    await _tripService.UpdateTripStatusAsync(trip.Id, TripStatus.Cancelled);
                    cancelled++;

                    await _notifier.NotifyEmailAsync(
                        trip.CustomerEmail,
                        $"Trip #{trip.Id} cancelled automatically",
                        $"Your trip booked for {trip.TripDate:dd MMM yyyy HH:mm} was never confirmed and has been cancelled. Please book again if you still need it.",
                        $"/CustomerPortal/Trip/Details/{trip.Id}");
                }
                catch (Exception ex)
                {
                    // Keep going: one unreachable trip should not block the rest of the sweep.
                    _logger.LogError(ex, "Failed to auto-cancel stale trip {TripId}", trip.Id);
                }
            }

            if (cancelled > 0)
            {
                await _notifier.NotifyRolesAsync(
                    $"{cancelled} stale trip(s) cancelled",
                    $"Trips left pending more than {_options.StaleTripGraceHours} hour(s) past their trip time were cancelled automatically.",
                    "/Travel/Trip",
                    UserRole.TravelManager);
            }

            return $"{stale.Count} stale trip(s) found, {cancelled} cancelled";
        }
    }
}
