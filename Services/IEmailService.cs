namespace Cab_Management_System.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
        Task SendBookingConfirmationAsync(string to, string customerName, string route, DateTime tripDate, decimal cost);
        Task SendPasswordResetAsync(string to, string resetLink);
        Task SendTripStatusUpdateAsync(string to, string customerName, int tripId, string newStatus);
    }
}
