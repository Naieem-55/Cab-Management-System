namespace Cab_Management_System.Repositories
{
    public interface INotificationRepository : IRepository<Models.Notification>
    {
        Task<IEnumerable<Models.Notification>> GetByUserIdAsync(string userId, int count = 20);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
    }
}
