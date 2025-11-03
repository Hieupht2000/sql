using CarManagetment.Config;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Globalization;

namespace CarManagetment.Services.Invoices
{
    public interface IEmailServiceCustomer
    {
        Task SendInvoiceCustomer
            (
                string toemail,
                string customername,
                int invoiceId,
                DateTime DateIssued,
                decimal TotalAmount,
                decimal VAT,
                string servicename,
                decimal PaymentStatus
            );
    }
    public class EmailServiceCustomer : IEmailServiceCustomer
    {
        private readonly ILogger<EmailServiceCustomer> _logger;
        private readonly EmailSettings _emailSettings;
        public EmailServiceCustomer(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public EmailServiceCustomer(ILogger<EmailServiceCustomer> logger, IOptions<EmailSettings> emailSettings)
        {
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }
        public async Task SendInvoiceCustomer
            (
                string toemail, 
                string customername, 
                int invoiceId, 
                DateTime DateIssued, 
                decimal TotalAmount,
                decimal VAT,
                string servicename, 
                decimal PaymentStatus
            )
        {
            var template = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "InvoiceCustomer.html");
            var html = await File.ReadAllTextAsync(template);
            html = html.Replace("{{InvoiceId}}", invoiceId.ToString())
                       .Replace("{{FullName}}", customername ?? "")
                       .Replace("{{DateIssued}}", DateIssued.ToString("yyyy-MM-dd"))
                       .Replace("{{TotalAmount}}", TotalAmount.ToString("C0", CultureInfo.GetCultureInfo("vi-VN")))
                       .Replace("{{VAT}}", VAT.ToString("N2"))
                       .Replace("{{ServiceName}}", servicename ?? "")
                       .Replace("{{PaymentStatus}}", PaymentStatus == 0 ? "Unpaid" : "Paid");
            _logger.LogInformation("Preparing to send invoice email to {Email}", toemail);
            await Task.Delay(100); // Simulate async work
            Console.WriteLine($"Sending invoice to {toemail}");
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress("", toemail));
            message.Subject = "Your Invoice";
            message.Body = new TextPart(TextFormat.Html) { Text = html };
            using var client = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                _logger.LogInformation("Invoice email sent successfully to {Email}", toemail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send invoice email to {Email}", toemail);
            }
        }
    }
}
