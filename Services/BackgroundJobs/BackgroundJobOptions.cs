namespace CabManagementSystem.Services.BackgroundJobs
{
    /// <summary>
    /// Settings for the scheduled jobs, bound from the "BackgroundJobs" configuration section.
    /// Every value has a working default, so no configuration is required.
    /// </summary>
    public class BackgroundJobOptions
    {
        public const string SectionName = "BackgroundJobs";

        /// <summary>Master switch. Set to false to keep the runner idle (useful for local debugging).</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>Grace period after startup before the first tick, so the app finishes booting first.</summary>
        public TimeSpan StartupDelay { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>How often the runner wakes up to look for jobs that are due.</summary>
        public TimeSpan PollInterval { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>Run every job once on the first tick instead of waiting out its interval.</summary>
        public bool RunOnStartup { get; set; } = true;

        /// <summary>Repeat notifications about the same subject no more often than this.</summary>
        public TimeSpan NotificationDedupeWindow { get; set; } = TimeSpan.FromHours(20);

        public TimeSpan LicenseExpiryInterval { get; set; } = TimeSpan.FromHours(12);

        /// <summary>Warn this many days before a driving licence expires.</summary>
        public int LicenseExpiryWarningDays { get; set; } = 30;

        public TimeSpan MaintenanceDueInterval { get; set; } = TimeSpan.FromHours(12);

        /// <summary>Warn this many days before maintenance falls due.</summary>
        public int MaintenanceDueWarningDays { get; set; } = 7;

        public TimeSpan StaleTripInterval { get; set; } = TimeSpan.FromHours(1);

        /// <summary>Cancel trips still Pending this many hours after their trip time.</summary>
        public int StaleTripGraceHours { get; set; } = 24;

        public TimeSpan PromoExpiryInterval { get; set; } = TimeSpan.FromHours(6);
    }
}
