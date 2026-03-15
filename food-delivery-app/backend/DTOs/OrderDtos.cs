namespace FoodDeliveryAPI.DTOs
{
    public class PlaceOrderRequest
    {
        public int AddressId { get; set; }
        public string PaymentMethod { get; set; } = "CashOnDelivery";
    }

    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; } = "Pending";
    }
}
