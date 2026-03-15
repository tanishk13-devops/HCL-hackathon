using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Repositories.Interfaces;
using FoodDeliveryAPI.Services.Interfaces;

namespace FoodDeliveryAPI.Services.Implementations
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;

        public RestaurantService(IRestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

        public async Task<List<Restaurant>> GetRestaurantsAsync(string? search)
            => await _restaurantRepository.GetAllAsync(search);

        public async Task<Restaurant?> GetRestaurantAsync(int id, bool includeMenu = false)
            => await _restaurantRepository.GetByIdAsync(id, includeMenu);

        public async Task<Restaurant> CreateRestaurantAsync(RestaurantCreateRequest request)
        {
            var restaurant = new Restaurant
            {
                Name = request.Name,
                Description = request.Description,
                Location = request.Location,
                Rating = request.Rating,
                ImageUrl = request.ImageUrl
            };

            return await _restaurantRepository.AddAsync(restaurant);
        }

        public async Task<bool> UpdateRestaurantAsync(int id, RestaurantUpdateRequest request)
        {
            var current = await _restaurantRepository.GetByIdAsync(id);
            if (current == null) return false;

            current.Name = request.Name;
            current.Description = request.Description;
            current.Location = request.Location;
            current.Rating = request.Rating;
            current.ImageUrl = request.ImageUrl;

            return await _restaurantRepository.UpdateAsync(current);
        }

        public async Task<bool> DeleteRestaurantAsync(int id)
            => await _restaurantRepository.DeleteAsync(id);
    }
}
