using CarManagetment.Data;
using CarManagetment.DTOs;
using CarManagetment.Model;
using Microsoft.EntityFrameworkCore;

namespace CarManagetment.Services.Notification
{
    public class TelegramNotificationService : INotificationService
    {
        private readonly TelegramService _telegramService;
        private CarDBContext _context;

        public TelegramNotificationService(TelegramService telegramService, CarDBContext context)
        {
            _telegramService = telegramService;
            _context = context;
        }

        public async Task SendNotificationAsync(InvoiceDTO invoiceDTO)
        {
            var booking = await _context.Booking
                .FirstOrDefaultAsync(b => b.Booking_id == invoiceDTO.BookingId);
            Customer customer = null;
            if (booking != null)
            {
                customer = await _context.Customer
                    .FirstOrDefaultAsync(c => c.CustomerId == booking.CustomerId);
            }
            string message = $"New Invoice Created:\n" +
                             $"Invoice ID: {invoiceDTO.InvoiceId}\n" +
                             $"Customer: {customer?.FullName ?? "N/A"}\n" +
                             $"Email: {invoiceDTO.Email}\n" +
                             $"Date Issued: {invoiceDTO.DateIssued:d}\n" +
                             $"Total Amount: {invoiceDTO.TotalAmount:N0}\n" +
                             $"Services: {(invoiceDTO.InvoiceDetails != null && invoiceDTO.InvoiceDetails.Count > 0
                                         ? invoiceDTO.InvoiceDetails[0].ServiceName
                                         : "N/A")}\n" +
                             $"Payment Status: {invoiceDTO.PaymentStatus}";
            await _telegramService.SendMessageAsync(message);
        }

        public async Task SendAsync(string message)
        {
            await _telegramService.SendMessageAsync(message);
        }
    }
}
