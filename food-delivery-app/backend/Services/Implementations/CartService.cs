using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Repositories.Interfaces;
using FoodDeliveryAPI.Services.Interfaces;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryAPI.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly FoodDeliveryDbContext _context;

        public CartService(ICartRepository cartRepository, IFoodRepository foodRepository, FoodDeliveryDbContext context)
        {
            _cartRepository = cartRepository;
            _foodRepository = foodRepository;
            _context = context;
        }

        public async Task<Cart> GetCartAsync(int userId)
            => await _cartRepository.GetOrCreateByUserIdAsync(userId);

        public async Task<Cart> AddItemAsync(int userId, AddCartItemRequest request)
        {
            var cart = await _cartRepository.GetOrCreateByUserIdAsync(userId);
            var food = await _foodRepository.GetByIdAsync(request.FoodItemId);

            if (food == null || !food.IsAvailable)
            {
                throw new InvalidOperationException("Food item is unavailable.");
            }

            var item = cart.CartItems.FirstOrDefault(i => i.FoodItemId == request.FoodItemId);
            if (item == null)
            {
                cart.CartItems.Add(new CartItem
                {
                    FoodItemId = request.FoodItemId,
                    Quantity = Math.Max(1, request.Quantity),
                    Price = food.Price
                });
            }
            else
            {
                item.Quantity += Math.Max(1, request.Quantity);
            }

            await _cartRepository.SaveChangesAsync();
            return await _cartRepository.GetOrCreateByUserIdAsync(userId);
        }

        public async Task<Cart?> UpdateItemAsync(int userId, int foodItemId, UpdateCartItemRequest request)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null) return null;

            var item = cart.CartItems.FirstOrDefault(i => i.FoodItemId == foodItemId);
            if (item == null) return cart;

            if (request.Quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = request.Quantity;
            }

            await _cartRepository.SaveChangesAsync();
            return await _cartRepository.GetByUserIdAsync(userId);
        }

        public async Task<Cart?> RemoveItemAsync(int userId, int foodItemId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null) return null;

            var item = cart.CartItems.FirstOrDefault(i => i.FoodItemId == foodItemId);
            if (item == null) return cart;

            _context.CartItems.Remove(item);
            await _cartRepository.SaveChangesAsync();
            return await _cartRepository.GetByUserIdAsync(userId);
        }
    }
}
