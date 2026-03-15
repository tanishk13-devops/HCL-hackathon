using FoodDeliveryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryAPI.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IRestaurantService _restaurantService;

        public AdminController(IOrderService orderService, IRestaurantService restaurantService)
        {
            _orderService = orderService;
            _restaurantService = restaurantService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var restaurants = await _restaurantService.GetRestaurantsAsync(null);

            var customers = orders.Select(o => o.UserId).Distinct().Count();
            var revenue = orders.Where(o => o.Status == "Delivered").Sum(o => o.TotalAmount);

            return Ok(new
            {
                totalRestaurants = restaurants.Count,
                totalOrders = orders.Count,
                totalCustomers = customers,
                deliveredRevenue = revenue,
                activeOrders = orders.Where(o => o.Status != "Delivered").Count()
            });
        }

        [HttpGet("orders")]
        public async Task<IActionResult> Orders()
            => Ok(await _orderService.GetAllOrdersAsync());

        [HttpGet("customer-activity")]
        public async Task<IActionResult> CustomerActivity()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var data = orders
                .GroupBy(o => new { o.UserId, Name = o.User!.Name })
                .Select(g => new
                {
                    userId = g.Key.UserId,
                    customerName = g.Key.Name,
                    orderCount = g.Count(),
                    spent = g.Sum(x => x.TotalAmount)
                })
                .OrderByDescending(x => x.orderCount)
                .ToList();

            return Ok(data);
        }
    }
}
