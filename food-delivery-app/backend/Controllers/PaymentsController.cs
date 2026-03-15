using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("order/{orderId:int}")]
        public async Task<IActionResult> GetByOrder(int orderId)
        {
            var payment = await _paymentService.GetByOrderAsync(orderId);
            return payment == null ? NotFound() : Ok(payment);
        }

        [HttpPatch("order/{orderId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] PaymentUpdateRequest request)
        {
            var payment = await _paymentService.UpdateStatusAsync(orderId, request.PaymentStatus);
            return payment == null ? BadRequest("Invalid order or payment status.") : Ok(payment);
        }
    }
}
