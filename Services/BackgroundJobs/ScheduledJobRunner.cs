using Microsoft.Extensions.Options;

namespace CabManagementSystem.Services.BackgroundJobs
{
    /// <summary>
    /// Single background loop that runs every registered <see cref="IScheduledJob"/> on its own interval.
    /// Each run gets a fresh DI scope, and a failing job never stops the loop or its siblings.
    /// </summary>
    public class ScheduledJobRunner : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly BackgroundJobOptions _options;
        private readonly ILogger<ScheduledJobRunner> _logger;
        private readonly Dictionary<string, DateTime> _lastRun = new();

        public ScheduledJobRunner(
            IServiceScopeFactory scopeFactory,
            IOptions<BackgroundJobOptions> options,
            ILogger<ScheduledJobRunner> logger)
        {
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.Enabled)
            {
                _logger.LogInformation("Background jobs are disabled; scheduler will not start.");
                return;
            }

            try
            {
                await Task.Delay(_options.StartupDelay, stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await RunDueJobsAsync(stoppingToken);
                    await Task.Delay(_options.PollInterval, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown.
            }

            _logger.LogInformation("Background job scheduler stopped.");
        }

        private async Task RunDueJobsAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var jobs = scope.ServiceProvider.GetServices<IScheduledJob>();

            foreach (var job in jobs)
            {
                if (stoppingToken.IsCancellationRequested)
                    return;

                if (!IsDue(job))
                    continue;

                _lastRun[job.Name] = DateTime.Now;

                try
                {
                    var summary = await job.RunAsync(stoppingToken);
                    _logger.LogInformation("Job {Job} finished: {Summary}", job.Name, summary);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    // One bad job must not take down the scheduler or block the others.
                    _logger.LogError(ex, "Job {Job} failed", job.Name);
                }
            }
        }

        private bool IsDue(IScheduledJob job)
        {
            if (!_lastRun.TryGetValue(job.Name, out var last))
                return _options.RunOnStartup;

            return DateTime.Now - last >= job.Interval;
        }
    }
}
