using FoodDeliveryAPI.Data;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryAPI.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly FoodDeliveryDbContext _context;

        public UserRepository(FoodDeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByIdAsync(int userId)
            => await _context.Users.FindAsync(userId);

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
