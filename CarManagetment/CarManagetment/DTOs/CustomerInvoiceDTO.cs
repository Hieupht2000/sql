namespace CarManagetment.DTOs
{
    public class CustomerInvoiceDTO
    {
        public int InvoiceId { get; set; }
        public int BookingId { get; set; }
        public DateTime DateIssued { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
    }
}
