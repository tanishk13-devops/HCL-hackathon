using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Repositories.Interfaces;
using FoodDeliveryAPI.Services.Interfaces;

namespace FoodDeliveryAPI.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<Review> AddReviewAsync(int userId, ReviewRequest request)
        {
            var review = new Review
            {
                UserId = userId,
                RestaurantId = request.RestaurantId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            return await _reviewRepository.AddAsync(review);
        }

        public async Task<List<Review>> GetByRestaurantAsync(int restaurantId)
            => await _reviewRepository.GetByRestaurantIdAsync(restaurantId);
    }
}
