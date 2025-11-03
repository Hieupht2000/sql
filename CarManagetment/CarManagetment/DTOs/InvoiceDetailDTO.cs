using System.ComponentModel.DataAnnotations;

namespace CarManagetment.DTOs
{
    public class InvoiceDetailDTO
    {

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        public int InvoiceDetailId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }   // để hiển thị
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
