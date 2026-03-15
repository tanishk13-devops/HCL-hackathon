using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
