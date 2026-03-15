using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Services.Interfaces;

namespace FoodDeliveryAPI.Controllers
{
    [ApiController]
    [Route("api/food-items")]
    public class FoodsController : ControllerBase
    {
        private readonly IFoodService _foodService;

        public FoodsController(IFoodService foodService)
        {
            _foodService = foodService;
        }

        [HttpGet("restaurant/{restaurantId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Food>>> GetRestaurantMenu(int restaurantId, [FromQuery] int? categoryId)
        {
            return Ok(await _foodService.GetRestaurantMenuAsync(restaurantId, categoryId));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Food>> GetFood(int id)
        {
            var food = await _foodService.GetByIdAsync(id);
            if (food == null)
                return NotFound();

            return food;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Food>> CreateFood([FromBody] FoodItemRequest request)
        {
            var food = await _foodService.CreateAsync(request);
            return CreatedAtAction(nameof(GetFood), new { id = food.Id }, food);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFood(int id, [FromBody] FoodItemRequest request)
        {
            var ok = await _foodService.UpdateAsync(id, request);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFood(int id)
        {
            var ok = await _foodService.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
