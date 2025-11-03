using CarManagetment.DTOs;
using Hangfire;

namespace CarManagetment.Services.Invoices
{
    public class EmailInvoice
    {
        private readonly IEmailServiceCustomer _emailServiceCustomer;
        private readonly ILogger<EmailInvoice> _logger;
        public EmailInvoice(IEmailServiceCustomer emailServiceCustomer, ILogger<EmailInvoice> logger)
        {
            _emailServiceCustomer = emailServiceCustomer;
            _logger = logger;
        }

        public async Task SendInvoiceToCustomerAsync(InvoiceDTO invoiceDTO)
        {

            //await _emailServiceCustomer.SendInvoiceCustomer
            //    (
            //        toemail: invoiceDTO.Email,
            //        customername: invoiceDTO.FullName,
            //        invoiceId: invoiceDTO.InvoiceId,
            //        DateIssued: invoiceDTO.DateIssued,
            //        TotalAmount: invoiceDTO.TotalAmount,
            //        VAT: invoiceDTO.VAT = Math.Round(invoiceDTO.VAT, 0),
            //        servicename: invoiceDTO.InvoiceDetails != null && invoiceDTO.InvoiceDetails.Count > 0 ? invoiceDTO.InvoiceDetails[0].ServiceName : "N/A",
            //        PaymentStatus: invoiceDTO.PaymentStatus == "Paid" ? 1 : 0
            //    );
            if (invoiceDTO == null)
            {
                _logger.LogError("InvoiceDTO is null. Cannot send invoice email.");
                return;
            }

            if(string.IsNullOrEmpty(invoiceDTO.Email))
            {
                _logger.LogError("Customer email is null or empty. Cannot send invoice email.");
                return;
            }

            _logger.LogInformation("Enqueuing job to send invoice email to {Email}", invoiceDTO.Email);


            BackgroundJob.Enqueue<IEmailServiceCustomer>(emailService =>
                emailService.SendInvoiceCustomer(
                    invoiceDTO.Email,
                    invoiceDTO.FullName,
                    invoiceDTO.InvoiceId,
                    invoiceDTO.DateIssued,
                    invoiceDTO.TotalAmount,
                    invoiceDTO.VAT,
                    invoiceDTO.InvoiceDetails != null && invoiceDTO.InvoiceDetails.Count > 0
                        ? invoiceDTO.InvoiceDetails[0].ServiceName
                        : "N/A",
                    invoiceDTO.PaymentStatus == "Paid" ? 1 :
                0
            ));
            await Task.CompletedTask;
        }
    }
}
