namespace CarManagetment.Model
{
    public class Invoice
    {
        
        public int InvoiceId { get; set; }
        public int Booking_id { get; set; }
        public DateTime DateIssued { get; set; } = DateTime.Now;
        public decimal VAT { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } = "Unpaid";

        // Navigation property
        public Booking Booking { get; set; }
        public ICollection<InvoiceDetail> InvoiceDetails { get; set; }
    }
}
