namespace FoodDeliveryAPI.Helpers
{
    public static class PaymentConstants
    {
        public static readonly string[] Methods = { "UPI", "Card", "CashOnDelivery" };
        public static readonly string[] Statuses = { "Pending", "Paid", "Failed", "Refunded" };
    }
}
