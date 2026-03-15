using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Repositories.Interfaces
{
    public interface IRestaurantRepository
    {
        Task<List<Restaurant>> GetAllAsync(string? search);
        Task<Restaurant?> GetByIdAsync(int id, bool includeMenu = false);
        Task<Restaurant> AddAsync(Restaurant restaurant);
        Task<bool> UpdateAsync(Restaurant restaurant);
        Task<bool> DeleteAsync(int id);
    }
}
