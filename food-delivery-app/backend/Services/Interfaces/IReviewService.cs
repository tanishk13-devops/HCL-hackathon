using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Services.Interfaces
{
    public interface IReviewService
    {
        Task<Review> AddReviewAsync(int userId, ReviewRequest request);
        Task<List<Review>> GetByRestaurantAsync(int restaurantId);
    }
}
