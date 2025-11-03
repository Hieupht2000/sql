using CarManagetment.Model;
using System.ComponentModel.DataAnnotations;

namespace CarManagetment.DTOs
{
    public class BookingDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public int Booking_id { get; set; }
        public int CustomerId { get; set; }
        public string ? FullName { get; set; }
        public int CarId { get; set; }
        public string LiensePlate { get; set; }
        public int  ? TechnicianId { get; set; }
        public string TechnicianName { get; set; }
        public int GarageId { get; set; }
        public DateTime BookingDate { get; set; }
        public int TimeSlot_Id { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]

        [AllowedValues("pending", "confirmed", "completed")]
        public string? Status { get; set; } = "pending";// e.g., "Pending", "Confirmed", "completed"
        public string Note { get; set; } // Optional note for the booking
        public string? EmailBody { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string dealerEmail { get; set; }
        public string? ServiceName { get; set; }

        //public List<Customer> Customers { get; set; }
    }
}
