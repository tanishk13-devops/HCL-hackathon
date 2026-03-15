using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Repositories.Interfaces
{
    public interface IAddressRepository
    {
        Task<List<Address>> GetByUserIdAsync(int userId);
        Task<Address?> GetByIdAsync(int addressId);
        Task<Address> AddAsync(Address address);
        Task<bool> DeleteAsync(int addressId, int userId);
    }
}
