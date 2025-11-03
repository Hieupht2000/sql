using System.ComponentModel.DataAnnotations;

namespace CarManagetment.Model
{
    public class Garages
    {
        [Key]
        public int GarageId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}
