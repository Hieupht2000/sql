namespace CarManagetment.Model
{
    public class InvoiceDetail
    {
        
            public int InvoiceDetailId { get; set; }
            public int InvoiceId { get; set; }
            public int ServiceId { get; set; }
            public int Quantity { get; set; } = 1;
            public decimal UnitPrice { get; set; }

            // Computed property (chỉ đọc, không map DB nếu dùng EF Core < 7)
            public decimal LineTotal => Quantity * UnitPrice;

            // Navigation
            public Invoice Invoice { get; set; }
            public Servicess Service { get; set; }
    }
}
