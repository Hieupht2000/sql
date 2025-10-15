using CarManagetment.Data;
using CarManagetment.DTOs;
using CarManagetment.Model;
using CarManagetment.Services.Invoices;
using CarManagetment.Services.Notification;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarManagetment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly CarDBContext _context;
        private readonly IInvoiceService _invoiceService;
        private readonly INotificationService _notificationService;
        private readonly EmailInvoice _emailInvoice;
        private readonly PdfInvoiceService _pdfService;

        public InvoicesController(CarDBContext carDBContext,IInvoiceService invoiceService, INotificationService notificationService, PdfInvoiceService pdfService)
        {

            _context = carDBContext;
            _invoiceService = invoiceService;
            _notificationService = notificationService;
            _pdfService = pdfService;

        }

        //GET: api/Invoices/get all
        [HttpGet("get all")]    
        public async Task<IActionResult> GetAllInvoices()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return Ok(invoices);
        }

        //GET: api/Invoices/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid invoice ID." });
                var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
                if (invoice == null) return NotFound(new {message = "$ Invoice with ID {id} not found"});
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }

        //POST: api/Invoices
        [Authorize (Roles ="admin")]
        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDTO cdto)
        {
            try
            { 
                var invoice = await _invoiceService.CreateInvoiceAsync(cdto);
                return CreatedAtAction(nameof(GetInvoiceById), new { id = invoice.InvoiceId }, invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        //PUT: api/Invoices/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] InvoiceDTO dto)
        {
            try
            {
                var result = await _invoiceService.UpdateInvoiceAsync(id, dto);
                if (!result) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }

        //DELETE: api/Invoices/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            try
            {
                
                var result = await _invoiceService.DeleteInvoiceAsync(id);
                if (!result) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("notify")]
        public async Task<IActionResult> SendTestMessage()
        {
            await _notificationService.SendAsync("🚗 Hello from Car Management API!");
            return Ok("Sent Telegram notification!");
        }

        //GET: api/Invoices/{id}/pdf
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GetInvoicePdf(int id)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
                if (invoice == null) return NotFound();
                var pdfBytes = _pdfService.GeneratePdfInvoice(invoice);
                return File(pdfBytes, "application/pdf", $"Invoice_{invoice.InvoiceId}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Post: api/Invoices/SendEmailComfrimed
        [HttpPost("SendEmailComfrimed")]
        public async Task<IActionResult> SendInvoiceEmail(int InvoiceId)
        {
            try
            {
                var invoice = _context.Invoices
                    .Include(i => i.InvoiceDetails)
                        .ThenInclude(i => i.Service)
                    .Include(i => i.Booking)
                        .ThenInclude(b => b.Customer)
                    .FirstOrDefault(i => i.InvoiceId == InvoiceId);

                if (invoice == null)
                    return NotFound(new { message = "Invoice not found." });

                // Map the Invoice entity to InvoiceDTO
                var invoiceDTO = new InvoiceDTO
                {
                    InvoiceId = invoice.InvoiceId,
                    FullName = invoice.Booking.FullName,
                    Email = invoice.Booking.Customer.Email,
                    DateIssued = invoice.DateIssued,
                    TotalAmount = invoice.TotalAmount,
                    VAT = invoice.VAT,
                    InvoiceDetails = invoice.InvoiceDetails?.Select(d => new InvoiceDetailDTO
                    {
                        ServiceName = d.Service.Name,
                        UnitPrice = d.UnitPrice
                    }).ToList()
                };
                
                BackgroundJob.Enqueue<EmailInvoice>(emailInvoice =>
                        emailInvoice.SendInvoiceToCustomerAsync(invoiceDTO));

                return Ok(new { message = "Email sent successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // POST: api/Invoices/SendNotification
        [HttpPost("SendTelegram/invoiceId")]

        public async Task<IActionResult> SendInvoiceNotification(int InvoiceId)
        {
            try
            {
                var invoice = _context.Invoices
                                   .Include(i => i.InvoiceDetails)
                                       .ThenInclude(i => i.Service)
                                   .Include(i => i.Booking)
                                       .ThenInclude(b => b.Customer)
                                   .FirstOrDefault(i => i.InvoiceId == InvoiceId);

                if (invoice == null)
                    return NotFound(new { message = "Invoice not found." });

                // Map the Invoice entity to InvoiceDTO
                var invoiceDTO = new InvoiceDTO
                {
                    InvoiceId = invoice.InvoiceId,
                    FullName = invoice.Booking.FullName,
                    Email = invoice.Booking.Customer.Email,
                    DateIssued = invoice.DateIssued,
                    TotalAmount = invoice.TotalAmount,
                    VAT = invoice.VAT,
                    InvoiceDetails = invoice.InvoiceDetails?.Select(d => new InvoiceDetailDTO
                    {
                        ServiceName = d.Service.Name,
                        UnitPrice = d.UnitPrice
                    }).ToList()
                };
                
                await _notificationService.SendNotificationAsync(invoiceDTO);
                return Ok(new { message = "Notification sent successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

