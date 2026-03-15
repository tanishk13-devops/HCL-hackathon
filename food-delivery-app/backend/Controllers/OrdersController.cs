using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Helpers;
using FoodDeliveryAPI.Services.Interfaces;

namespace FoodDeliveryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,DeliveryAgent")]
        public async Task<IActionResult> GetOrders()
        {
            return Ok(await _orderService.GetAllOrdersAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
                return NotFound();

            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("DeliveryAgent");
            if (!isAdmin && order.UserId != userId)
            {
                return Forbid();
            }

            return Ok(order);
        }

        [HttpGet("my-history")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> GetOrdersByCurrentCustomer()
        {
            var userId = User.GetUserId();
            return Ok(await _orderService.GetOrderHistoryAsync(userId));
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var order = await _orderService.PlaceOrderAsync(userId, request);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,DeliveryAgent")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest statusUpdate)
        {
            var ok = await _orderService.UpdateStatusAsync(id, statusUpdate.Status);
            if (!ok) return BadRequest("Invalid transition or order not found.");

            return NoContent();
        }

        [HttpGet("{id}/track")]
        public async Task<IActionResult> TrackOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            return Ok(new
            {
                order.Id,
                order.Status,
                Workflow = Helpers.OrderWorkflow.All,
                order.CreatedAt
            });
        }
    }
}
