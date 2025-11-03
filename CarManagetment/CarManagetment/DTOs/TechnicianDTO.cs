namespace CarManagetment.DTOs
{
    public class TechnicianDTO
    {
        public int TechnicianId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Status { get; set; } // e.g., "Available", "Busy", "On Leave"
    }
}
