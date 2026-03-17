namespace FoodDeliveryAPI.Services.Interfaces
{
    public interface IFoodImageService
    {
        Task<string> GetFoodImageUrlAsync(string foodName, CancellationToken cancellationToken = default);
    }
}
