using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryAPI.Models
{
    public class Restaurant
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Name { get; set; } = string.Empty;

        [StringLength(600)]
        public string Description { get; set; } = string.Empty;

        [Required, StringLength(160)]
        public string Location { get; set; } = string.Empty;

        [Range(0, 5)]
        public decimal Rating { get; set; } = 4.2m;

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Food> FoodItems { get; set; } = new List<Food>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
