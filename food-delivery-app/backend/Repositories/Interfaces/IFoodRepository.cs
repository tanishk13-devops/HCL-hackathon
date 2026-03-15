using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Repositories.Interfaces
{
    public interface IFoodRepository
    {
        Task<List<Food>> GetByRestaurantAsync(int restaurantId, int? categoryId = null);
        Task<Food?> GetByIdAsync(int id);
        Task<Food> AddAsync(Food food);
        Task<bool> UpdateAsync(Food food);
        Task<bool> DeleteAsync(int id);
    }
}
