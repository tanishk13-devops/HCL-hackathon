using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Services.Interfaces
{
    public interface IAddressService
    {
        Task<List<Address>> GetAllAsync(int userId);
        Task<Address> CreateAsync(int userId, AddressRequest request);
        Task<bool> DeleteAsync(int userId, int addressId);
    }
}
