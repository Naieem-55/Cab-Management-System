using CabManagementSystem.Models.Enums;
using CabManagementSystem.Repositories;
using Microsoft.Extensions.Options;

namespace CabManagementSystem.Services.BackgroundJobs
{
    /// <summary>
    /// Flags maintenance whose next service date has passed as Overdue, and warns travel
    /// managers about work that is due soon or already late.
    /// </summary>
    public class MaintenanceDueJob : IScheduledJob
    {
        private readonly IMaintenanceRepository _maintenanceRepository;
        private readonly JobNotifier _notifier;
        private readonly BackgroundJobOptions _options;

        public MaintenanceDueJob(
            IMaintenanceRepository maintenanceRepository,
            JobNotifier notifier,
            IOptions<BackgroundJobOptions> options)
        {
            _maintenanceRepository = maintenanceRepository;
            _notifier = notifier;
            _options = options.Value;
        }

        public string Name => "MaintenanceDue";

        public TimeSpan Interval => _options.MaintenanceDueInterval;

        public async Task<string> RunAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var horizon = now.AddDays(_options.MaintenanceDueWarningDays);

            var records = (await _maintenanceRepository.GetDueMaintenanceAsync(horizon)).ToList();

            var flagged = 0;
            var notified = 0;

            foreach (var record in records)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var due = record.NextMaintenanceDate!.Value;
                var isOverdue = due < now;

                // Keep the status in step with the calendar so list views and dashboards agree.
                if (isOverdue && record.Status != MaintenanceStatus.Overdue)
                {
                    record.Status = MaintenanceStatus.Overdue;
                    _maintenanceRepository.Update(record);
                    flagged++;
                }

                var vehicle = record.Vehicle?.RegistrationNumber ?? $"Vehicle #{record.VehicleId}";
                var title = isOverdue
                    ? $"Maintenance overdue: {vehicle}"
                    : $"Maintenance due soon: {vehicle}";
                var message = isOverdue
                    ? $"Service was due on {due:dd MMM yyyy}: {record.Description}"
                    : $"Service is due on {due:dd MMM yyyy}: {record.Description}";

                notified += await _notifier.NotifyRolesAsync(
                    title, message, $"/Travel/Maintenance/Details/{record.Id}",
                    UserRole.TravelManager, UserRole.Admin);
            }

            if (flagged > 0)
                await _maintenanceRepository.SaveChangesAsync();

            return $"{records.Count} record(s) due within {_options.MaintenanceDueWarningDays} days, {flagged} marked overdue, {notified} notification(s) sent";
        }
    }
}
