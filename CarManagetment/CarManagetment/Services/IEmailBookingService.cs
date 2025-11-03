using CarManagetment.Config;
using CarManagetment.Model;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Net.Mail;

namespace CarManagetment.Services
{
    public interface IEmailBookingService
    {
        Task SendBookingConfirmationAsync(string toEmail, int customerId, string customerName, DateTime bookingDate, string licensePlate, string technicianName, string body);
        Task SendBookingNotificationToDealerAsync(string dealerEmail, string garageName, string customerName, int customerId, string customerEmail, string licensePlate, DateTime bookingDate, string serviceName);
        Task SendNotificationReminderAsync(string email, string fullName, DateTime bookingDate, string licensePlate);
    }

    public class EmailService : IEmailBookingService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public EmailService(ILogger<EmailService> logger, IOptions<EmailSettings> emailSettings)
        {
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }

        public async Task SendBookingConfirmationAsync(string toEmail, int customerId, string customerName, DateTime bookingDate, string licensePlate, string technicianName, string body = null)
        {
            var template = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "BookingConfirmation.html");
            var html = await File.ReadAllTextAsync(template);

            html = html.Replace("{{FullName}}", customerName ?? "")
                       .Replace("{{BookingDate}}", bookingDate.ToString("yyyy-MM-dd HH:mm"))
                       .Replace("{{LicensePlate}}", licensePlate ?? "")
                       .Replace("{{Technician}}", technicianName ?? "Unassigned")
                       .Replace("{{CustomerId}}", customerId.ToString());

            if (!string.IsNullOrEmpty(body))
            {
                html = html.Replace("{{CustomBody}}",body);
            }
            else
            {
                html = html.Replace("{{CustomBody}}", "");
            }
            _logger.LogInformation("Preparing to send booking confirmation email to {Email}", toEmail);
            await Task.Delay(100); // Simulate async work
            Console.WriteLine($"Sending booking confirmation to {toEmail}");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress(customerName ?? "Customer", toEmail));
            message.Subject = "Booking Confirmation";
            message.Body = new TextPart(TextFormat.Html) { Text = html };
            
            using var client = new MailKit.Net.Smtp.SmtpClient();
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        public async Task SendNotificationReminderAsync(string email, string fullName, DateTime bookingDate, string licensePlate)
        {
            var template = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "BookingReminderTemplate.html");
            var html = await File.ReadAllTextAsync(template);
            html = html.Replace("{{FullName}}", fullName ?? "")
                       .Replace("{{BookingDate}}", bookingDate.ToString("yyyy-MM-dd HH:mm"))
                       .Replace("{{LicensePlate}}", licensePlate ?? "");
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress(fullName, email));
            message.Subject = "Booking Reminder";
            message.Body = new TextPart(TextFormat.Html) { Text = html };
            using var client = new MailKit.Net.Smtp.SmtpClient();
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        public async Task SendBookingNotificationToDealerAsync
            (
            string dealerEmail, 
            string garageName,
            string customerName,
            int customerId,
            string customerEmail,
            string licensePlate,
            DateTime bookingDate,
            string serviceName
            )
        
        {
            var template = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "NewBookingNotification.html");
            var html = await File.ReadAllTextAsync(template);
            html = html.Replace("{{GarageName}}", garageName ?? "")
                       .Replace("{{CustomerName}}", customerName ?? "")
                       .Replace("{{CustomerId}}", customerId.ToString())
                       .Replace("{{CustomerEmail}}", customerEmail ?? "")
                       .Replace("{{LicensePlate}}", licensePlate ?? "")
                       .Replace("{{BookingDate}}", bookingDate.ToString("yyyy-MM-dd HH:mm"))
                       .Replace("{{ServiceName}}", serviceName ?? "");
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress(garageName, dealerEmail));
            message.Subject = "New Booking Notification";
            message.Body = new TextPart(TextFormat.Html) { Text = html };
            using var client = new MailKit.Net.Smtp.SmtpClient();
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        
    }
}
