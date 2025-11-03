using CarManagetment.DTOs;

namespace CarManagetment.Services.Invoices
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceDTO>> GetAllInvoicesAsync();
        Task<InvoiceDTO> GetInvoiceByIdAsync(int invoiceId);
        Task<InvoiceDTO> CreateInvoiceAsync(CreateInvoiceDTO dto);
        Task<bool> UpdateInvoiceAsync(int invoiceId, InvoiceDTO invoiceDto);
        Task<bool> DeleteInvoiceAsync(int invoiceId);
        Task<IEnumerable<InvoiceDTO>> GetInvoicesByCustomerAsync(int customerId);

    }
}
