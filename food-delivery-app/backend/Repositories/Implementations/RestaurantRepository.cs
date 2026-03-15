using FoodDeliveryAPI.Data;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryAPI.Repositories.Implementations
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly FoodDeliveryDbContext _context;

        public RestaurantRepository(FoodDeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<List<Restaurant>> GetAllAsync(string? search)
        {
            var query = _context.Restaurants.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r => r.Name.Contains(search) || r.Location.Contains(search));
            }

            return await query.OrderByDescending(r => r.Rating).ToListAsync();
        }

        public async Task<Restaurant?> GetByIdAsync(int id, bool includeMenu = false)
        {
            var query = _context.Restaurants.AsQueryable();
            if (includeMenu)
            {
                query = query
                    .Include(r => r.FoodItems)
                    .ThenInclude(f => f.Category);
            }

            return await query.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Restaurant> AddAsync(Restaurant restaurant)
        {
            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();
            return restaurant;
        }

        public async Task<bool> UpdateAsync(Restaurant restaurant)
        {
            var exists = await _context.Restaurants.AnyAsync(r => r.Id == restaurant.Id);
            if (!exists) return false;

            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Restaurants.FindAsync(id);
            if (entity == null) return false;

            _context.Restaurants.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
