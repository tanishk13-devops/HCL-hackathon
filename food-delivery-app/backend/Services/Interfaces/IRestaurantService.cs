using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Services.Interfaces
{
    public interface IRestaurantService
    {
        Task<List<Restaurant>> GetRestaurantsAsync(string? search);
        Task<Restaurant?> GetRestaurantAsync(int id, bool includeMenu = false);
        Task<Restaurant> CreateRestaurantAsync(RestaurantCreateRequest request);
        Task<bool> UpdateRestaurantAsync(int id, RestaurantUpdateRequest request);
        Task<bool> DeleteRestaurantAsync(int id);
    }
}
