using Cab_Management_System.Models;

namespace Cab_Management_System.Services
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId, int count = 20);
        Task<int> GetUnreadCountAsync(string userId);
        Task CreateNotificationAsync(string userId, string title, string message, string? link = null);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
    }
}
