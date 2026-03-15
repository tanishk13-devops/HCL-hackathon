namespace FoodDeliveryAPI.DTOs
{
    public class ReviewRequest
    {
        public int RestaurantId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
