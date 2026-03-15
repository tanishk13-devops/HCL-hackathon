using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Repositories.Interfaces;
using FoodDeliveryAPI.Services.Interfaces;

namespace FoodDeliveryAPI.Services.Implementations
{
    public class FoodService : IFoodService
    {
        private readonly IFoodRepository _foodRepository;

        public FoodService(IFoodRepository foodRepository)
        {
            _foodRepository = foodRepository;
        }

        public async Task<List<Food>> GetRestaurantMenuAsync(int restaurantId, int? categoryId)
            => await _foodRepository.GetByRestaurantAsync(restaurantId, categoryId);

        public async Task<Food?> GetByIdAsync(int id)
            => await _foodRepository.GetByIdAsync(id);

        public async Task<Food> CreateAsync(FoodItemRequest request)
        {
            var food = new Food
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CategoryId = request.CategoryId,
                RestaurantId = request.RestaurantId,
                ImageUrl = request.ImageUrl,
                IsAvailable = request.IsAvailable,
                CreatedAt = DateTime.UtcNow
            };

            return await _foodRepository.AddAsync(food);
        }

        public async Task<bool> UpdateAsync(int id, FoodItemRequest request)
        {
            var food = await _foodRepository.GetByIdAsync(id);
            if (food == null) return false;

            food.Name = request.Name;
            food.Description = request.Description;
            food.Price = request.Price;
            food.CategoryId = request.CategoryId;
            food.RestaurantId = request.RestaurantId;
            food.ImageUrl = request.ImageUrl;
            food.IsAvailable = request.IsAvailable;

            return await _foodRepository.UpdateAsync(food);
        }

        public async Task<bool> DeleteAsync(int id)
            => await _foodRepository.DeleteAsync(id);
    }
}
