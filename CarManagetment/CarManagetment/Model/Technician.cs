using System.ComponentModel.DataAnnotations;

namespace CarManagetment.Model
{
    public class Technician
    {
        [Key]
        public int TechnicianId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Status { get; set; } // e.g., "Available", "Busy", "On Leave"
    }
}
