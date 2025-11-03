using CarManagetment.Data;
using CarManagetment.DTOs;
using CarManagetment.Model;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarManagetment.Services
{
    public class BookingService : IBookingService
    {
        private readonly CarDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EmailService _emailService;
        public BookingService(CarDBContext context, IHttpContextAccessor httpContextAccessor, EmailService emailService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;
            return userIdClaim != null ? int.Parse(userIdClaim) : 0; // Default to 0 if not found
        }
        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Booking.ToListAsync();
        }

        //public async Task<Booking> CreateAsync(Booking booking)
        //{
        //    _context.Booking.Add(booking);
        //    await _context.SaveChangesAsync();
        //    return booking;
        //}
        public async Task<Booking> CreateBookingAsync(BookingDTO bookingdtos, ClaimsPrincipal user)
        {
            try
            {
                //var userId = int.Parse(user.FindFirst("user_id").Value); // Get the username from the ClaimsPrincipal

                //var customer = await _context.Customer
                //    .FirstOrDefaultAsync(c => c.CustomerId == bookingdtos.CustomerId);

                //if (customer == null)
                //{
                //    throw new Exception("Customer not found for the provided username.");
                //}
              
                bool isLicensePlateBooked = await _context.Booking
                .AnyAsync(b => b.LicensePlate == bookingdtos.LiensePlate
                            && b.BookingDate.Date == bookingdtos.BookingDate.Date);

                if (isLicensePlateBooked)
                    throw new Exception("This license plate is already booked for the selected date.");

                // Check if the time slot is already taken for the specified garage and date
                bool isSlotTaken = await _context.Booking
                .AnyAsync(b => b.TimeSlot_Id == bookingdtos.TimeSlot_Id
                            && b.BookingDate.Date == bookingdtos.BookingDate.Date
                            && b.GarageId == bookingdtos.GarageId);

                if (isSlotTaken)
                    throw new Exception("Time slot already booked for this garage.");

                var booking = new Booking
                {
                    CustomerId = bookingdtos.CustomerId,//set UserId to from token for customerId
                    FullName = bookingdtos.FullName,
                    CarId = bookingdtos.CarId,
                    LicensePlate = bookingdtos.LiensePlate,
                    TechnicianId = null,
                    GarageId = bookingdtos.GarageId,
                    BookingDate = bookingdtos.BookingDate,
                    TimeSlot_Id = bookingdtos.TimeSlot_Id,
                    Status = string.IsNullOrEmpty(bookingdtos.Status) ? "Pending" : bookingdtos.Status, // Default status
                    Note = bookingdtos.Note ?? string.Empty // Optional note, default to empty string if null
                };
                _context.Booking.Add(booking);
                await _context.SaveChangesAsync();
    
                return booking;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"Failed to save booking: {innerExceptionMessage}");
            }
        }
        public async Task<Booking> AssignTechnicianAsync(int bookingId, int technicianId)
        {
            try
            {
                var booking = await _context.Booking.FindAsync(bookingId);
                if (booking == null) throw new Exception("Booking not found.");

                var technician = await _context.technician.FindAsync(technicianId);
                if (technician == null) throw new Exception("Technician not found.");
                // 1. Free technical check
                bool isBusy = await _context.Booking
                    .AnyAsync(b => b.TechnicianId == technicianId
                                && b.BookingDate.Date == booking.BookingDate.Date
                                && b.Booking_id != bookingId); // Exclude the current booking

                if (isBusy)
                    throw new Exception("Technician already assigned to another booking at this time.");
                booking.TechnicianId = technicianId;
                await _context.SaveChangesAsync();
                return booking;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"Failed to assign technician: {innerExceptionMessage}");
            }
        }

        public async Task<Booking> GetByIdAsync(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null) throw new Exception("Booking not found.");
            return booking;
        }

        public async Task UpdateBookingStatus(int id, string status, string note, int? operatorTechnicianId = null)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null) throw new Exception("Booking not found.");
            booking.Status = status;
            booking.Note = note ; // Ensure Note is not null
            if (operatorTechnicianId.HasValue)
            {
                booking.TechnicianId = operatorTechnicianId.Value; // Update technician if provided
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBooking(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null) throw new Exception("Booking not found.");
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }
}
