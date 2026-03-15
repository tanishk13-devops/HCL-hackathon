using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetOrCreateByUserIdAsync(int userId);
        Task<Cart?> GetByUserIdAsync(int userId);
        Task SaveChangesAsync();
    }
}
