using FoodDeliveryAPI.Data;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryAPI.Repositories.Implementations
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly FoodDeliveryDbContext _context;

        public PaymentRepository(FoodDeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> AddAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> GetByOrderIdAsync(int orderId)
            => await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
