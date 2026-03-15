using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryAPI.Models
{
    public class FoodCategory
    {
        public int Id { get; set; }

        [Required, StringLength(80)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Food> FoodItems { get; set; } = new List<Food>();
    }
}
