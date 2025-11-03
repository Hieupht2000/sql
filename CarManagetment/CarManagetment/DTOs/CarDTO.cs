namespace CarManagetment.DTOs
{
    public class CarDTO
    {
        public int CarId { get; set; }
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string LicensePlate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Odometer { get; set; }

    }
}
