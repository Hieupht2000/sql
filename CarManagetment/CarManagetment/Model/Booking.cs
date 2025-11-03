using System.ComponentModel.DataAnnotations;

namespace CarManagetment.Model
{
    public class Booking
    {
        [Key]
        public int Booking_id { get; set; }
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public int CarId { get; set; }
        public string LicensePlate { get; set; }
        public int ? TechnicianId { get; set; }
        //public string FullName_Technial { get; set; }
        public int GarageId { get; set; }
        public DateTime BookingDate { get; set; }
        public int TimeSlot_Id { get; set; }

        [AllowedValues("pending", "confirmed", "completed")]
        public string? Status { get; set; } = "pending"; // e.g., "Pending", "Confirmed", "completed"

        public string Note { get; set; } // Optional note for the booking

        //Navigation properties
        public Customer Customer { get; set; }
        //public Car Car { get; set; }
        public Technician Technician { get; set; }
        public Garages Garage { get; set; }

    }
}
