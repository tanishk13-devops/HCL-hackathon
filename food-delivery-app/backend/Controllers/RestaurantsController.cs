using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Data;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryAPI.Controllers
{
    [ApiController]
    [Route("api/restaurants")]
    public class RestaurantsController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly ILogger<RestaurantsController> _logger;
        private readonly FoodDeliveryDbContext _dbContext;

        public RestaurantsController(
            IRestaurantService restaurantService,
            ILogger<RestaurantsController> logger,
            FoodDeliveryDbContext dbContext)
        {
            _restaurantService = restaurantService;
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            try
            {
                var restaurants = await _restaurantService.GetRestaurantsAsync(search);

                if (restaurants.Count == 0)
                {
                    await EnsureMinimalRestaurantsAsync();
                    restaurants = await _restaurantService.GetRestaurantsAsync(search);
                }

                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch restaurants. Returning empty fallback list.");
                return Ok(new List<Restaurant>());
            }
        }

        private async Task EnsureMinimalRestaurantsAsync()
        {
            if (await _dbContext.Restaurants.AnyAsync())
            {
                return;
            }

            var now = DateTime.UtcNow;
            var seedRestaurants = new List<Restaurant>
            {
                new()
                {
                    Name = "Spice Route",
                    Description = "North Indian favorites and biryanis.",
                    Location = "Noida",
                    Rating = 4.4m,
                    ImageUrl = "https://images.unsplash.com/photo-1552566626-52f8b828add9?q=80&w=1200",
                    CreatedAt = now
                },
                new()
                {
                    Name = "Coastal Bowl",
                    Description = "South Indian and coastal comfort food.",
                    Location = "Gurgaon",
                    Rating = 4.3m,
                    ImageUrl = "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?q=80&w=1200",
                    CreatedAt = now
                },
                new()
                {
                    Name = "Urban Biryani House",
                    Description = "Layered biryanis and mughlai delicacies.",
                    Location = "Delhi",
                    Rating = 4.2m,
                    ImageUrl = "https://images.unsplash.com/photo-1481833761820-0509d3217039?q=80&w=1200",
                    CreatedAt = now
                }
            };

            _dbContext.Restaurants.AddRange(seedRestaurants);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Seeded minimal restaurant catalog on demand.");
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var restaurant = await _restaurantService.GetRestaurantAsync(id, includeMenu: true);
            return restaurant == null ? NotFound() : Ok(restaurant);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] RestaurantCreateRequest request)
        {
            var restaurant = await _restaurantService.CreateRestaurantAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = restaurant.Id }, restaurant);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] RestaurantUpdateRequest request)
        {
            var ok = await _restaurantService.UpdateRestaurantAsync(id, request);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _restaurantService.DeleteRestaurantAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
