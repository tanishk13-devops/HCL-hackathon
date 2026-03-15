using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review> AddAsync(Review review);
        Task<List<Review>> GetByRestaurantIdAsync(int restaurantId);
        Task SaveChangesAsync();
    }
}
