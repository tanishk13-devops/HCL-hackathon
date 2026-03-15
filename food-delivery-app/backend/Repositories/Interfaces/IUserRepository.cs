using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int userId);
        Task<User> AddAsync(User user);
    }
}
