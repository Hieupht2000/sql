using CarManagetment.Services.Invoices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarManagetment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerInvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<CustomerInvoiceController> _logger;
        public CustomerInvoiceController(IInvoiceService invoiceService, ILogger<CustomerInvoiceController> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        // GET: api/customer/invoices/customer/5
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetInvoicesByCustomer(int customerId)
        {
            var invoices = await _invoiceService.GetInvoicesByCustomerAsync(customerId);
            if (invoices == null || !invoices.Any())
                return NotFound(new { message = "No invoices found for this customer." });

            return Ok(invoices);
        }

        //// GET: api/customer/invoices/5
        //[Authorize]
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetInvoiceForCustomerById(int id)
        //{
        //    int customerId = GetCurrentCustomerId(); // lấy từ JWT
        //    var invoice = await _invoiceService.GetInvoiceForCustomerByIdAsync(customerId, id);
        //    if (invoice == null)
        //    {
        //        _logger.LogWarning($"Invoice with ID {id} not found for customer {customerId}");
        //        return NotFound(new { message = "Invoice not found" });
        //    }

        //    return Ok(invoice);
        //}

        [Authorize(Roles = "customer")]
        [HttpGet("my-invoices")]
        public async Task<IActionResult> GetMyInvoices(int customerId)
        {
            var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (customerIdClaim == null)
            {
                _logger.LogWarning("Customer ID claim not found in token");
                return Unauthorized(new { message = "Invalid token: Customer ID missing." });
            }

            var customerid = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation($"Customer ID from token: {customerIdClaim}");

            var invoices = await _invoiceService.GetInvoicesByCustomerAsync(customerId);
            if (invoices == null || !invoices.Any())
                return NotFound(new { message = "You don't have any invoices." });

            return Ok(invoices);
        }


        [Authorize]
        [HttpGet("debug/claims")]
        public IActionResult DebugClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }


        //// Ví dụ lấy userID từ JWT token
        //private int GetCurrentCustomerId()
        //{
        //    var claim = User.FindFirst("userId");
        //    if (claim == null)
        //    {
        //        throw new UnauthorizedAccessException("Token does not contain userId claim");
        //    }
        //    return int.Parse(claim.Value);
        //}

    }
}
