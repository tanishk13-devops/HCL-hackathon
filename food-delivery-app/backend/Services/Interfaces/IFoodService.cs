using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Services.Interfaces
{
    public interface IFoodService
    {
        Task<List<Food>> GetRestaurantMenuAsync(int restaurantId, int? categoryId);
        Task<Food?> GetByIdAsync(int id);
        Task<Food> CreateAsync(FoodItemRequest request);
        Task<bool> UpdateAsync(int id, FoodItemRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
