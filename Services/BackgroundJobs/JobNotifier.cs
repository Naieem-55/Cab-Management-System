using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CabManagementSystem.Services.BackgroundJobs
{
    /// <summary>
    /// Sends deduplicated in-app notifications on behalf of scheduled jobs.
    /// Jobs re-run on a schedule, so every notification goes through the dedupe window.
    /// </summary>
    public class JobNotifier
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BackgroundJobOptions _options;

        public JobNotifier(
            INotificationService notificationService,
            UserManager<ApplicationUser> userManager,
            IOptions<BackgroundJobOptions> options)
        {
            _notificationService = notificationService;
            _userManager = userManager;
            _options = options.Value;
        }

        /// <summary>Notifies every user in the given roles. Returns how many notifications were created.</summary>
        public async Task<int> NotifyRolesAsync(string title, string message, string? link, params UserRole[] roles)
        {
            var created = 0;

            foreach (var role in roles)
            {
                var users = await _userManager.GetUsersInRoleAsync(role.ToString());
                foreach (var user in users)
                {
                    if (await _notificationService.CreateIfNotRecentAsync(
                            user.Id, title, message, link, _options.NotificationDedupeWindow))
                    {
                        created++;
                    }
                }
            }

            return created;
        }

        /// <summary>Notifies the account owning this email address, when one exists.</summary>
        public async Task<bool> NotifyEmailAsync(string? email, string title, string message, string? link)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            return await _notificationService.CreateIfNotRecentAsync(
                user.Id, title, message, link, _options.NotificationDedupeWindow);
        }
    }
}
