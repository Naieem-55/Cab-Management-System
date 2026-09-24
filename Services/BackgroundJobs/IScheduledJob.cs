namespace CabManagementSystem.Services.BackgroundJobs
{
    /// <summary>
    /// A unit of recurring work executed by <see cref="ScheduledJobRunner"/>.
    /// Implementations are registered as scoped services and resolved once per run.
    /// </summary>
    public interface IScheduledJob
    {
        /// <summary>Stable name used for scheduling and log messages.</summary>
        string Name { get; }

        /// <summary>How often this job should run.</summary>
        TimeSpan Interval { get; }

        /// <summary>Does the work and returns a one-line summary for the log.</summary>
        Task<string> RunAsync(CancellationToken cancellationToken);
    }
}
