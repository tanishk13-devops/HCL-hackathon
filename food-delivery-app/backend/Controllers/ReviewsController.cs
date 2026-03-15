using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Helpers;
using FoodDeliveryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryAPI.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("restaurant/{restaurantId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRestaurantReviews(int restaurantId)
            => Ok(await _reviewService.GetByRestaurantAsync(restaurantId));

        [HttpPost]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> Add([FromBody] ReviewRequest request)
            => Ok(await _reviewService.AddReviewAsync(User.GetUserId(), request));
    }
}
