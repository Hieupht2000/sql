using CarManagetment.DTOs;
using CarManagetment.Model;
using System.Security.Claims;

namespace CarManagetment.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<Booking> CreateBookingAsync(BookingDTO bookingdtos,ClaimsPrincipal user);
        Task<Booking> AssignTechnicianAsync(int bookingId, int technicianId);
        Task<Booking> GetByIdAsync(int id);
        Task UpdateBookingStatus(int id, string Status, string note, int? operatorTechnicianId =null);
        Task DeleteBooking(int id);
    }
}
