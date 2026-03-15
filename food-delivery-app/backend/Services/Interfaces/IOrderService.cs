using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> PlaceOrderAsync(int userId, PlaceOrderRequest request);
        Task<List<Order>> GetOrderHistoryAsync(int userId);
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<bool> UpdateStatusAsync(int id, string status);
    }
}
