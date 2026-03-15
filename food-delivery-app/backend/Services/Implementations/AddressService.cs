using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Repositories.Interfaces;
using FoodDeliveryAPI.Services.Interfaces;

namespace FoodDeliveryAPI.Services.Implementations
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;

        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<List<Address>> GetAllAsync(int userId)
            => await _addressRepository.GetByUserIdAsync(userId);

        public async Task<Address> CreateAsync(int userId, AddressRequest request)
        {
            var address = new Address
            {
                UserId = userId,
                Street = request.Street,
                City = request.City,
                State = request.State,
                Pincode = request.Pincode
            };

            return await _addressRepository.AddAsync(address);
        }

        public async Task<bool> DeleteAsync(int userId, int addressId)
            => await _addressRepository.DeleteAsync(addressId, userId);
    }
}
