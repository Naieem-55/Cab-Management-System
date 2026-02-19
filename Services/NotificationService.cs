using Cab_Management_System.Models;
using Cab_Management_System.Repositories;

namespace Cab_Management_System.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(INotificationRepository notificationRepository, ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId, int count = 20)
        {
            return await _notificationRepository.GetByUserIdAsync(userId, count);
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _notificationRepository.GetUnreadCountAsync(userId);
        }

        public async Task CreateNotificationAsync(string userId, string title, string message, string? link = null)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    Link = link,
                    IsRead = false
                };

                await _notificationRepository.AddAsync(notification);
                await _notificationRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification for user {UserId}", userId);
            }
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            await _notificationRepository.MarkAsReadAsync(notificationId);
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            await _notificationRepository.MarkAllAsReadAsync(userId);
        }
    }
}
