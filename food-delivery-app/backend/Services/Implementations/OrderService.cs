using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Helpers;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Repositories.Interfaces;
using FoodDeliveryAPI.Services.Interfaces;
using FoodDeliveryAPI.Data;

namespace FoodDeliveryAPI.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly FoodDeliveryDbContext _context;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IAddressRepository addressRepository,
            IPaymentRepository paymentRepository,
            FoodDeliveryDbContext context)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _addressRepository = addressRepository;
            _paymentRepository = paymentRepository;
            _context = context;
        }

        public async Task<Order> PlaceOrderAsync(int userId, PlaceOrderRequest request)
        {
            if (!PaymentConstants.Methods.Contains(request.PaymentMethod))
            {
                throw new InvalidOperationException("Unsupported payment method.");
            }

            var address = await _addressRepository.GetByIdAsync(request.AddressId);
            if (address == null || address.UserId != userId)
            {
                throw new InvalidOperationException("Address not found.");
            }

            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                throw new InvalidOperationException("Cart is empty.");
            }

            var order = new Order
            {
                UserId = userId,
                AddressId = request.AddressId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                TotalAmount = cart.CartItems.Sum(i => i.Price * i.Quantity),
                Items = cart.CartItems.Select(ci => new OrderItem
                {
                    FoodItemId = ci.FoodItemId,
                    Quantity = ci.Quantity,
                    Price = ci.Price
                }).ToList()
            };

            await _orderRepository.AddAsync(order);

            var payment = new Payment
            {
                OrderId = order.Id,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = request.PaymentMethod == "CashOnDelivery" ? "Pending" : "Paid",
                Amount = order.TotalAmount,
                CreatedAt = DateTime.UtcNow
            };
            await _paymentRepository.AddAsync(payment);

            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();

            return await _orderRepository.GetByIdAsync(order.Id) ?? order;
        }

        public async Task<List<Order>> GetOrderHistoryAsync(int userId)
            => await _orderRepository.GetByUserIdAsync(userId);

        public async Task<List<Order>> GetAllOrdersAsync()
            => await _orderRepository.GetAllAsync();

        public async Task<Order?> GetOrderByIdAsync(int id)
            => await _orderRepository.GetByIdAsync(id);

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            if (!OrderWorkflow.IsValid(status)) return false;

            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return false;
            if (!OrderWorkflow.CanTransition(order.Status, status)) return false;

            order.Status = status;
            await _orderRepository.SaveChangesAsync();
            return true;
        }
    }
}
