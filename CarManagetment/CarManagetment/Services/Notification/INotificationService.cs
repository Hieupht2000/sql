using CarManagetment.DTOs;

namespace CarManagetment.Services.Notification
{
    public interface INotificationService
    {
        Task SendNotificationAsync(InvoiceDTO invoiceDTO);
        Task SendAsync(string message);
    }
}
