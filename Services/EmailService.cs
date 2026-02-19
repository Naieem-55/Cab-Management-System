using Cab_Management_System.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Cab_Management_System.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.SmtpHost))
                {
                    _logger.LogWarning("SMTP not configured. Email to {To} with subject '{Subject}' was not sent.", to, subject);
                    return;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, _settings.EnableSsl);

                if (!string.IsNullOrWhiteSpace(_settings.SmtpUsername))
                {
                    await client.AuthenticateAsync(_settings.SmtpUsername, _settings.SmtpPassword);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent to {To} with subject '{Subject}'", to, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To} with subject '{Subject}'", to, subject);
            }
        }

        public async Task SendBookingConfirmationAsync(string to, string customerName, string route, DateTime tripDate, decimal cost)
        {
            var subject = "Booking Confirmation - Cab Management System";
            var htmlBody = $@"
                <h2>Booking Confirmation</h2>
                <p>Dear {customerName},</p>
                <p>Your trip has been booked successfully. Here are the details:</p>
                <table style='border-collapse: collapse; width: 100%;'>
                    <tr><td style='padding: 8px; border: 1px solid #ddd;'><strong>Route</strong></td><td style='padding: 8px; border: 1px solid #ddd;'>{route}</td></tr>
                    <tr><td style='padding: 8px; border: 1px solid #ddd;'><strong>Trip Date</strong></td><td style='padding: 8px; border: 1px solid #ddd;'>{tripDate:MMM dd, yyyy HH:mm}</td></tr>
                    <tr><td style='padding: 8px; border: 1px solid #ddd;'><strong>Cost</strong></td><td style='padding: 8px; border: 1px solid #ddd;'>{cost:C}</td></tr>
                </table>
                <p>Thank you for choosing our service!</p>
                <p><em>Cab Management System</em></p>";

            await SendEmailAsync(to, subject, htmlBody);
        }

        public async Task SendTripStatusUpdateAsync(string to, string customerName, int tripId, string newStatus)
        {
            var subject = $"Trip #{tripId} Status Update - Cab Management System";
            var htmlBody = $@"
                <h2>Trip Status Update</h2>
                <p>Dear {customerName},</p>
                <p>Your trip <strong>#{tripId}</strong> status has been updated to: <strong>{newStatus}</strong></p>
                <p>Log in to your account to view full trip details.</p>
                <p>Thank you for choosing our service!</p>
                <p><em>Cab Management System</em></p>";

            await SendEmailAsync(to, subject, htmlBody);
        }

        public async Task SendPasswordResetAsync(string to, string resetLink)
        {
            var subject = "Password Reset - Cab Management System";
            var htmlBody = $@"
                <h2>Password Reset Request</h2>
                <p>You have requested a password reset. Click the link below to reset your password:</p>
                <p><a href='{resetLink}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Reset Password</a></p>
                <p>If you did not request this, please ignore this email.</p>
                <p>This link will expire after use.</p>
                <p><em>Cab Management System</em></p>";

            await SendEmailAsync(to, subject, htmlBody);
        }
    }
}
