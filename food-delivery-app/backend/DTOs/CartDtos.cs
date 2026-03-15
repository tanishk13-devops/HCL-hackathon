namespace FoodDeliveryAPI.DTOs
{
    public class AddCartItemRequest
    {
        public int FoodItemId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemRequest
    {
        public int Quantity { get; set; }
    }
}
