using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Helpers;
using FoodDeliveryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryAPI.Controllers
{
    [ApiController]
    [Authorize(Roles = "Customer,Admin")]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> ViewCart()
            => Ok(await _cartService.GetCartAsync(User.GetUserId()));

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemRequest request)
        {
            try
            {
                return Ok(await _cartService.AddItemAsync(User.GetUserId(), request));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("items/{foodItemId:int}")]
        public async Task<IActionResult> UpdateItem(int foodItemId, [FromBody] UpdateCartItemRequest request)
        {
            var cart = await _cartService.UpdateItemAsync(User.GetUserId(), foodItemId, request);
            return cart == null ? NotFound() : Ok(cart);
        }

        [HttpDelete("items/{foodItemId:int}")]
        public async Task<IActionResult> RemoveItem(int foodItemId)
        {
            var cart = await _cartService.RemoveItemAsync(User.GetUserId(), foodItemId);
            return cart == null ? NotFound() : Ok(cart);
        }
    }
}
