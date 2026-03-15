using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Services.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCartAsync(int userId);
        Task<Cart> AddItemAsync(int userId, AddCartItemRequest request);
        Task<Cart?> UpdateItemAsync(int userId, int foodItemId, UpdateCartItemRequest request);
        Task<Cart?> RemoveItemAsync(int userId, int foodItemId);
    }
}
