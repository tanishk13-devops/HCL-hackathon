namespace FoodDeliveryAPI.DTOs
{
    public class RestaurantCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal Rating { get; set; } = 4.2m;
        public string? ImageUrl { get; set; }
    }

    public class RestaurantUpdateRequest : RestaurantCreateRequest
    {
    }
}
