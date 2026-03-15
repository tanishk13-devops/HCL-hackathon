using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment?> GetByOrderAsync(int orderId);
        Task<Payment?> UpdateStatusAsync(int orderId, string paymentStatus);
    }
}
