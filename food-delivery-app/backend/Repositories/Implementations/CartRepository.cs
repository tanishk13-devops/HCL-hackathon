using FoodDeliveryAPI.Data;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryAPI.Repositories.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly FoodDeliveryDbContext _context;

        public CartRepository(FoodDeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByUserIdAsync(int userId)
            => await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.FoodItem)
                .FirstOrDefaultAsync(c => c.UserId == userId);

        public async Task<Cart> GetOrCreateByUserIdAsync(int userId)
        {
            var cart = await GetByUserIdAsync(userId);
            if (cart != null) return cart;

            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return await GetByUserIdAsync(userId) ?? cart;
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
