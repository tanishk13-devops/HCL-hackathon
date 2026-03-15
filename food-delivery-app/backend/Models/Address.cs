using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryAPI.Models
{
    public class Address
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required, StringLength(250)]
        public string Street { get; set; } = string.Empty;

        [Required, StringLength(80)]
        public string City { get; set; } = string.Empty;

        [Required, StringLength(80)]
        public string State { get; set; } = string.Empty;

        [Required, StringLength(12)]
        public string Pincode { get; set; } = string.Empty;

        public User? User { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
