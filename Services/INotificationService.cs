using CabManagementSystem.Models;

namespace CabManagementSystem.Services
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId, int count = 20);
        Task<int> GetUnreadCountAsync(string userId);
        Task CreateNotificationAsync(string userId, string title, string message, string? link = null);

        /// <summary>
        /// Creates a notification unless the same user already got one with this title inside the window.
        /// Lets recurring background jobs re-check their conditions without spamming the bell.
        /// Returns true when a notification was created.
        /// </summary>
        Task<bool> CreateIfNotRecentAsync(string userId, string title, string message, string? link, TimeSpan window);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
    }
}
