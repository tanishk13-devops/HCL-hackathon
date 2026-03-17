using FoodDeliveryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryAPI.Controllers
{
    [ApiController]
    [Route("api/food-images")]
    public class FoodImagesController : ControllerBase
    {
        private readonly IFoodImageService _foodImageService;

        public FoodImagesController(IFoodImageService foodImageService)
        {
            _foodImageService = foodImageService;
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Food name is required.");
            }

            var imageUrl = await _foodImageService.GetFoodImageUrlAsync(name, cancellationToken);
            return Ok(new { name, imageUrl });
        }
    }
}
