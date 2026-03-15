using FoodDeliveryAPI.Data;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryAPI.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly FoodDeliveryDbContext _context;

        public OrderRepository(FoodDeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<Order> AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetByUserIdAsync(int userId)
            => await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .ThenInclude(i => i.FoodItem)
                .Include(o => o.Address)
                .Include(o => o.Payment)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

        public async Task<List<Order>> GetAllAsync()
            => await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.FoodItem)
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.Payment)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

        public async Task<Order?> GetByIdAsync(int orderId)
            => await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.FoodItem)
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == orderId);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
