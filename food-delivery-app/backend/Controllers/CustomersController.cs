using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDeliveryAPI.Data;
using FoodDeliveryAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace FoodDeliveryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class CustomersController : ControllerBase
    {
        private readonly FoodDeliveryDbContext _context;

        public CustomersController(FoodDeliveryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetCustomers()
        {
            return await _context.Users
                .Where(u => u.Role == "Customer")
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetCustomer(int id)
        {
            var customer = await _context.Users
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Id == id && c.Role == "Customer");

            if (customer == null)
                return NotFound();

            return customer;
        }
    }
}
