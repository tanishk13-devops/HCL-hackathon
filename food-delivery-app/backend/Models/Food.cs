using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryAPI.Models
{
    public class Food
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public bool Availability { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
