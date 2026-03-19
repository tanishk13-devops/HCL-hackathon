using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryAPI.Controllers
{
    [ApiController]
    [Route("api/restaurants")]
    public class RestaurantsController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly ILogger<RestaurantsController> _logger;

        public RestaurantsController(
            IRestaurantService restaurantService,
            ILogger<RestaurantsController> logger)
        {
            _restaurantService = restaurantService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            try
            {
                return Ok(await _restaurantService.GetRestaurantsAsync(search));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch restaurants. Returning empty fallback list.");
                return Ok(new List<Restaurant>());
            }
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
