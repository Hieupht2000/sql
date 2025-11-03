using CarManagetment.Data;
using CarManagetment.DTOs;
using CarManagetment.Model;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace CarManagetment.Services.Invoices
{
    public class InvoiceService : IInvoiceService
    {
        private readonly CarDBContext _context;
        public InvoiceService(CarDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InvoiceDTO>> GetAllInvoicesAsync()
        {
            var invoices = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Service)
                .ToListAsync();
            return invoices.Select(i => new InvoiceDTO
            {
                InvoiceId = i.InvoiceId,
                BookingId = i.Booking_id,
                DateIssued = i.DateIssued,
                VAT = i.VAT,
                TotalAmount = i.TotalAmount,
                PaymentStatus = i.PaymentStatus,
                InvoiceDetails = i.InvoiceDetails.Select(d => new InvoiceDetailDTO
                {
                    InvoiceDetailId = d.InvoiceDetailId,
                    ServiceId = d.ServiceId,
                    ServiceName = d.Service.Name,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    LineTotal = d.LineTotal
                }).ToList()
            }).ToList();
        }

        public async Task<InvoiceDTO> GetInvoiceByIdAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Service)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null) return null;
           
            var booking = await _context.Booking
                .FirstOrDefaultAsync(b => b.Booking_id == invoice.Booking_id);
            Customer customer = null;
            if (booking != null)
            {
                customer = await _context.Customer
                    .FirstOrDefaultAsync(c => c.CustomerId == booking.CustomerId);
            }
            //Servicess service = null;
            return new InvoiceDTO
            {
                InvoiceId = invoice.InvoiceId,
                BookingId = invoice.Booking_id,
                DateIssued = invoice.DateIssued,
                VAT = invoice.VAT,
                TotalAmount = invoice.TotalAmount,
                PaymentStatus = invoice.PaymentStatus,
                Email = customer?.Email,
                FullName = customer != null ? $"{customer.FullName}" : "N/A",
                InvoiceDetails = invoice.InvoiceDetails.Select(d => new InvoiceDetailDTO
                {
                    InvoiceDetailId = d.InvoiceDetailId,
                    ServiceId = d.ServiceId,
                    ServiceName = d.Service.Name,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    LineTotal = d.LineTotal
                }).ToList()
            };
        }

        public async Task<InvoiceDTO> CreateInvoiceAsync(CreateInvoiceDTO invoiceDto)
        {
            var invoice = new Invoice
            {
                
                Booking_id = invoiceDto.Booking_id,
                DateIssued = DateTime.Now,
                VAT = invoiceDto.VAT,
                PaymentStatus = "Unpaid",
                InvoiceDetails = new List<InvoiceDetail>()
            };

            decimal subtotal = 0;

            foreach (var detailDto in invoiceDto.Services)
            {
                var service = await _context.Servicess.FindAsync(detailDto.ServiceId);
                if (service == null) continue; // Or handle error as needed

                var detail = new InvoiceDetail
                {
                    ServiceId = detailDto.ServiceId,
                    Quantity = detailDto.Quantity,
                    UnitPrice = service.Price

                };
                subtotal += detail.LineTotal;
                invoice.InvoiceDetails.Add(detail);
            }

            //VAT
            decimal vatAmount = Math.Round(subtotal + subtotal * invoice.VAT / 100M,2);

            //Total Amount 
            invoice.TotalAmount = Math.Round(subtotal + vatAmount, 2);
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return await GetInvoiceByIdAsync(invoice.InvoiceId);
        }

        public async Task<bool> UpdateInvoiceAsync(int invoiceid, InvoiceDTO invoiceDto)
        {
            var invoivce = await _context.Invoices.FindAsync(invoiceid);
            if (invoivce == null) return false;

            invoivce.VAT = invoiceDto.VAT;
            invoivce.TotalAmount = invoiceDto.TotalAmount;
            invoivce.PaymentStatus = invoiceDto.PaymentStatus;

            _context.Invoices.Update(invoivce);
            await _context.SaveChangesAsync();

            return true;

        }

        public async Task<bool> DeleteInvoiceAsync(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null) return false;

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<InvoiceDTO>> GetInvoicesByCustomerAsync(int customerId)
        {
            var invoices = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                        .ThenInclude(d => d.Service)
                .Include(i => i.Booking) // giả sử Invoice liên kết Booking
                .Where(i => i.Booking.CustomerId == customerId)
                .ToListAsync();

            return invoices.Select(MapToDTO);
        }

        public async Task<CustomerInvoiceDTO> GetInvoiceForCustomerByIdAsync(int customerId, int invoiceId)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId && i.Booking.CustomerId == customerId);

            if (invoice == null) return null;

            return new CustomerInvoiceDTO
            {
                InvoiceId = invoice.InvoiceId,
                BookingId = invoice.Booking_id,
                DateIssued = invoice.DateIssued,
                TotalAmount = invoice.TotalAmount,
                PaymentStatus = invoice.PaymentStatus
            };
        }

        // Trong InvoiceService hoặc tạo 1 InvoiceMapper class riêng
        private InvoiceDTO MapToDTO(Invoice invoice)
        {
            return new InvoiceDTO
            {
                InvoiceId = invoice.InvoiceId,
                BookingId = invoice.Booking_id,
                DateIssued = invoice.DateIssued,
                VAT = invoice.VAT,
                TotalAmount = invoice.TotalAmount,
                InvoiceDetails = invoice.InvoiceDetails?.Select(d => new InvoiceDetailDTO
                {
                    InvoiceDetailId = d.InvoiceDetailId,
                    ServiceId = d.ServiceId,
                    Quantity = d.Quantity,
                    LineTotal = d.LineTotal
                }).ToList()
            };
        }

    }
}
