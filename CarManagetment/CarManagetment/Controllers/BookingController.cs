using CarManagetment.Data;
using CarManagetment.DTOs;
using CarManagetment.Model;
using CarManagetment.Services;
using Hangfire;
using MailKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarManagetment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly BookingQueueService _bookingQueueService;
        private readonly ILogger<BookingController> _logger;
        private readonly CarDBContext _context;
        private readonly IEmailBookingService _emailSetting;
        public BookingController(IBookingService bookingService,BookingQueueService bookingQueueService, CarDBContext context, ILogger<BookingController> logger,IEmailBookingService emailService)
        {
            _bookingService = bookingService;
            _bookingQueueService = bookingQueueService;
            _context = context;
            _logger = logger;
            _emailSetting = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _bookingService.GetAllAsync();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var booking = await _bookingService.GetByIdAsync(id);
                if (booking == null)
                {
                    return NotFound(new { message = "Booking not found." });
                }
                return Ok(booking);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Create a new booking
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDTO dto)
        {
            try
            {
                var result = await _bookingService.CreateBookingAsync(dto, User);
                if (result == null)
                {
                    return BadRequest(new { message = "Failed to create booking." });
                }
                //await _bookingQueueService.SendImmediateNotificationsAsync(dto);
                //_bookingQueueService.ScheduleReminderEmail(dto);

                return Ok(new { message = "Booking created successfully", result });
                            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Assign technician to a booking
        [Authorize(Roles = "admin")]
        [HttpPut("{bookingId}/assign-technician/{technicianId}")]
        public async Task<IActionResult> AssignTechnician(int bookingId, int technicianId)
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                _logger.LogInformation($"User role from token: {userRole}");

                if (userRole != "Admin" && userRole != "admin")
                {
                    return StatusCode(403, new { message = "Chỉ admin mới có thể phân công kỹ thuật viên" });
                }
                var booking = await _bookingService.AssignTechnicianAsync(bookingId, technicianId);
                return Ok(new { message = "Technician assigned", booking });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning technician {BookingId}", bookingId);
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] string status, string note, int? operatorTechnicianId = null)
        {
            try
            {
                await _bookingService.UpdateBookingStatus(id, status, note, operatorTechnicianId);
                return Ok(new { message = "Booking status updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                await _bookingService.DeleteBooking(id);
                return Ok(new { message = "Booking deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //POST: api/booking/sendemailComfrimed
        [HttpPost("sendemailComfrimed/bookingId")]
        public async Task<IActionResult> SendEmailComfrimed(int bookingId)
        {
            try
            {
                var bookings = _context.Booking
                          .Include(b => b.Customer)
                          .Include(b => b.Garage)
                          .Include(b => b.Technician)
                       .FirstOrDefault(b => b.Booking_id == bookingId);

                if (bookings == null)
                {
                    return NotFound(new { message = "Booking not found." });
                }

                var customerEmail = bookings.Customer?.Email;
                
                if (string.IsNullOrEmpty(customerEmail))
                {
                    // Email không có → không gửi, trả về thông báo
                    _logger.LogWarning($"Booking {bookings.Booking_id} has no customer email.");
                    return BadRequest(new { message = "Customer email is missing or invalid." });
                }

                if (bookings.Customer == null || string.IsNullOrEmpty(bookings.Customer.Email))
                {
                    return BadRequest(new { message = "Customer email not found for this booking." });
                }

                var bookingDTO = new BookingDTO
                {
                    Booking_id = bookings.Booking_id,
                    CustomerId = bookings.CustomerId,
                    FullName = bookings.Customer?.FullName,
                    CarId = bookings.CarId,
                    LiensePlate = bookings.LicensePlate,
                    TechnicianId = bookings.TechnicianId,
                    TechnicianName = bookings.Technician?.FullName ?? "Unassigned",
                    GarageId = bookings.GarageId,
                    BookingDate = bookings.BookingDate,
                    TimeSlot_Id = bookings.TimeSlot_Id,
                    Status = bookings.Status,
                    Note = bookings.Note,
                    Email = bookings.Customer.Email,
                    //dealerEmail = bookings.Garage.Customer.Email,
                    ServiceName = "General Service" // Replace with actual service name if available

                };
                await _bookingQueueService.SendImmediateNotificationsAsync(bookingDTO);
                return Ok(new { message = "Email sent successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/booking/sendemailDealer
        [HttpPost("sendemailDealer/bookingId")]
        public async Task<IActionResult> SendEmailDealer(int bookingId,string emaildealer)
        {
            try
            {
                var bookings = _context.Booking
                          .Include(b => b.Customer)
                          .Include(b => b.Garage)
                          .Include(b => b.Technician)
                       .FirstOrDefault(b => b.Booking_id == bookingId);
                if (bookings == null)
                {
                    return NotFound(new { message = "Booking not found." });
                }
                var dealerEmail = bookings.Customer?.Email;
                if (string.IsNullOrEmpty(dealerEmail))
                {
                    // Email không có → không gửi, trả về thông báo
                    _logger.LogWarning($"Booking {bookings.Booking_id} has no dealer email.");
                    return BadRequest(new { message = "Dealer email is missing or invalid." });
                }
                if (bookings.Customer == null || string.IsNullOrEmpty(bookings.Customer.Email))
                {
                    return BadRequest(new { message = "Customer email not found for this booking." });
                }
                var bookingDTO = new BookingDTO
                {
                    Booking_id = bookings.Booking_id,
                    CustomerId = bookings.CustomerId,
                    FullName = bookings.Customer?.FullName,
                    CarId = bookings.CarId,
                    LiensePlate = bookings.LicensePlate,
                    TechnicianId = bookings.TechnicianId,
                    TechnicianName = bookings.Technician?.FullName ?? "Unassigned",
                    GarageId = bookings.GarageId,
                    BookingDate = bookings.BookingDate,
                    TimeSlot_Id = bookings.TimeSlot_Id,
                    Status = bookings.Status,
                    Note = bookings.Note,
                    Email = bookings.Customer.Email,
                    dealerEmail = bookings.Customer?.Email,
                    ServiceName = "General Service" // Replace with actual service name if available
                };
                await _bookingQueueService.SendEmailDealerAsync(bookingDTO);
                return Ok(new { message = "Email sent dealer successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // POST: api/booking/sendemailReminder
        [HttpPost("sendemailReminder/bookingId")]
        public async Task<IActionResult> SendEmailReminder(int bookingId)
        {
            try
            {
                var bookings = _context.Booking
                          .Include(b => b.Customer)
                          .Include(b => b.Garage)
                          .Include(b => b.Technician)
                       .FirstOrDefault(b => b.Booking_id == bookingId);
                if (bookings == null)
                {
                    return NotFound(new { message = "Booking not found." });
                }
                var customerEmail = bookings.Customer?.Email;
                if (string.IsNullOrEmpty(customerEmail))
                {
                    // Email không có → không gửi, trả về thông báo
                    _logger.LogWarning($"Booking {bookings.Booking_id} has no customer email.");
                    return BadRequest(new { message = "Customer email is missing or invalid." });
                }
                if (bookings.Customer == null || string.IsNullOrEmpty(bookings.Customer.Email))
                {
                    return BadRequest(new { message = "Customer email not found for this booking." });
                }
                var bookingDTO = new BookingDTO
                {
                    Email = bookings.Customer.Email,
                    CustomerId = bookings.CustomerId,
                    FullName = bookings.Customer?.FullName,
                    CarId = bookings.CarId,
                    BookingDate = bookings.BookingDate,
                    LiensePlate = bookings.LicensePlate,
                };
                //_bookingQueueService.ScheduleReminderEmail(bookingDTO);
                BackgroundJob.Schedule(
                    () => _emailSetting.SendNotificationReminderAsync(
                        bookingDTO.Email,
                        bookingDTO.FullName,
                        bookingDTO.BookingDate,
                        bookingDTO.LiensePlate
                    ),
                    TimeSpan.FromMinutes(5) // Schedule to run after 1 minute for testing
                );
                return Ok(new { message = "Email reminder scheduled successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
