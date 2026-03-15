using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Helpers;
using FoodDeliveryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryAPI.Controllers
{
    [ApiController]
    [Authorize(Roles = "Customer,Admin")]
    [Route("api/addresses")]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMine()
            => Ok(await _addressService.GetAllAsync(User.GetUserId()));

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddressRequest request)
            => Ok(await _addressService.CreateAsync(User.GetUserId(), request));

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _addressService.DeleteAsync(User.GetUserId(), id);
            return ok ? NoContent() : NotFound();
        }
    }
}
