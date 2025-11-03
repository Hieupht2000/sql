using System.ComponentModel.DataAnnotations;

namespace CarManagetment.DTOs
{
    public class InvoiceDTO
    {

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        public int InvoiceId { get; set; }
        public int BookingId { get; set; }
        public DateTime DateIssued { get; set; }
        public decimal VAT { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string ServiceName { get; set; }
        public List<InvoiceDetailDTO> InvoiceDetails { get; set; }
        public string FullName { get; set; }

    }
}
