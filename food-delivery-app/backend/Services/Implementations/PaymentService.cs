using FoodDeliveryAPI.Helpers;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Repositories.Interfaces;
using FoodDeliveryAPI.Services.Interfaces;

namespace FoodDeliveryAPI.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<Payment?> GetByOrderAsync(int orderId)
            => await _paymentRepository.GetByOrderIdAsync(orderId);

        public async Task<Payment?> UpdateStatusAsync(int orderId, string paymentStatus)
        {
            if (!PaymentConstants.Statuses.Contains(paymentStatus))
            {
                return null;
            }

            var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
            if (payment == null) return null;

            payment.PaymentStatus = paymentStatus;
            await _paymentRepository.SaveChangesAsync();
            return payment;
        }
    }
}
