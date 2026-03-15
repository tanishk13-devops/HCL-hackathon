using System.Security.Claims;

namespace FoodDeliveryAPI.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(value, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user context.");
            }

            return userId;
        }
    }
}
