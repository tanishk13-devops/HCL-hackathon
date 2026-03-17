using FoodDeliveryAPI.Services.Interfaces;
using System.Text.Json;

namespace FoodDeliveryAPI.Services.Implementations
{
    public class FoodImageService : IFoodImageService
    {
        private const string PlaceholderImage = "https://dummyimage.com/1200x800/111827/f9fafb&text=Food+Image";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FoodImageService> _logger;

        public FoodImageService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<FoodImageService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GetFoodImageUrlAsync(string foodName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(foodName))
            {
                return PlaceholderImage;
            }

            var normalized = foodName.Trim();

            // 0) Exact configured mapping (dish name -> image URL)
            var configuredImage = _configuration[$"FoodImage:SeedImageMap:{normalized}"];
            if (!string.IsNullOrWhiteSpace(configuredImage) && Uri.IsWellFormedUriString(configuredImage, UriKind.Absolute))
            {
                return configuredImage;
            }

            // 1) Pexels (if API key configured)
            var pexelsKey = _configuration["FoodImage:Providers:Pexels:ApiKey"];
            if (!string.IsNullOrWhiteSpace(pexelsKey))
            {
                var pexelsUrl = await TryGetFromPexelsAsync(normalized, pexelsKey, cancellationToken);
                if (!string.IsNullOrWhiteSpace(pexelsUrl)) return pexelsUrl;
            }

            // 2) Unsplash API (if access key configured)
            var unsplashKey = _configuration["FoodImage:Providers:Unsplash:AccessKey"];
            if (!string.IsNullOrWhiteSpace(unsplashKey))
            {
                var unsplashUrl = await TryGetFromUnsplashApiAsync(normalized, unsplashKey, cancellationToken);
                if (!string.IsNullOrWhiteSpace(unsplashUrl)) return unsplashUrl;
            }

            // 3) Deterministic fallback by dish name (always matches name)
            return BuildDishNameFallbackImageUrl(normalized);
        }

        private async Task<string?> TryGetFromPexelsAsync(string foodName, string apiKey, CancellationToken cancellationToken)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", apiKey);

                var query = Uri.EscapeDataString($"{foodName} plated food");
                var url = $"https://api.pexels.com/v1/search?query={query}&per_page=1&orientation=landscape";

                using var response = await client.GetAsync(url, cancellationToken);
                if (!response.IsSuccessStatusCode) return null;

                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

                if (!doc.RootElement.TryGetProperty("photos", out var photos) || photos.GetArrayLength() == 0)
                {
                    return null;
                }

                var src = photos[0].GetProperty("src");
                if (src.TryGetProperty("large2x", out var large2x))
                {
                    return large2x.GetString();
                }

                if (src.TryGetProperty("large", out var large))
                {
                    return large.GetString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Pexels image lookup failed for {FoodName}", foodName);
            }

            return null;
        }

        private async Task<string?> TryGetFromUnsplashApiAsync(string foodName, string accessKey, CancellationToken cancellationToken)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var query = Uri.EscapeDataString($"{foodName} plated food");
                var url = $"https://api.unsplash.com/search/photos?query={query}&orientation=landscape&content_filter=high&per_page=1&client_id={accessKey}";

                using var response = await client.GetAsync(url, cancellationToken);
                if (!response.IsSuccessStatusCode) return null;

                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

                if (!doc.RootElement.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
                {
                    return null;
                }

                var urls = results[0].GetProperty("urls");
                if (!urls.TryGetProperty("regular", out var regular)) return null;

                var baseUrl = regular.GetString();
                if (string.IsNullOrWhiteSpace(baseUrl)) return null;

                return baseUrl.Contains('?')
                    ? $"{baseUrl}&auto=format&fit=crop&w=1200&q=80"
                    : $"{baseUrl}?auto=format&fit=crop&w=1200&q=80";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unsplash API image lookup failed for {FoodName}", foodName);
            }

            return null;
        }

        private static string BuildDishNameFallbackImageUrl(string foodName)
        {
            var text = Uri.EscapeDataString(foodName);
            return $"https://dummyimage.com/1200x800/111827/f9fafb&text={text}";
        }
    }
}
