using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryAPI.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RestaurantId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(700)]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
        public Restaurant? Restaurant { get; set; }
    }
}
