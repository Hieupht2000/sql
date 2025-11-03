namespace CarManagetment.DTOs
{
    public class ServiceDTO
    {
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public TimeSpan EstimatedDuration { get; set; } // Duration in minutes
    }
}
