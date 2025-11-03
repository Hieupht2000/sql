using System.ComponentModel.DataAnnotations;

namespace CarManagetment.Model
{
    public class Customer
    {

        [Key]
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Booking Booking { get; set; }

        //public int user_id { get; set; } // Foreign key to the User table
        //public Users User { get; set; } // Navigation property to the User table
    }
}
