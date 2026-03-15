using FoodDeliveryAPI.Data;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryAPI.Repositories.Implementations
{
    public class FoodRepository : IFoodRepository
    {
        private readonly FoodDeliveryDbContext _context;

        public FoodRepository(FoodDeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<List<Food>> GetByRestaurantAsync(int restaurantId, int? categoryId = null)
        {
            var query = _context.Foods
                .Include(f => f.Category)
                .Where(f => f.RestaurantId == restaurantId && f.IsAvailable);

            if (categoryId.HasValue)
            {
                query = query.Where(f => f.CategoryId == categoryId.Value);
            }

            return await query.OrderBy(f => f.Name).ToListAsync();
        }

        public async Task<Food?> GetByIdAsync(int id)
            => await _context.Foods.Include(f => f.Category).FirstOrDefaultAsync(f => f.Id == id);

        public async Task<Food> AddAsync(Food food)
        {
            _context.Foods.Add(food);
            await _context.SaveChangesAsync();
            return food;
        }

        public async Task<bool> UpdateAsync(Food food)
        {
            var exists = await _context.Foods.AnyAsync(f => f.Id == food.Id);
            if (!exists) return false;

            _context.Foods.Update(food);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Foods.FindAsync(id);
            if (entity == null) return false;

            _context.Foods.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
