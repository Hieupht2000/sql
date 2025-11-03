namespace CarManagetment.DTOs
{
    public class CreateInvoiceDTO
    {
        public int Booking_id { get; set; }
        public decimal VAT { get; set; }
        public List<CreateInvoiceDetailDTO> Services { get; set; }
    }
}
