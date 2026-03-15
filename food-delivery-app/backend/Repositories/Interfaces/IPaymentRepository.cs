using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> AddAsync(Payment payment);
        Task<Payment?> GetByOrderIdAsync(int orderId);
        Task SaveChangesAsync();
    }
}
