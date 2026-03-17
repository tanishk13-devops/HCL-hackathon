using Microsoft.EntityFrameworkCore;
using FoodDeliveryAPI.Data;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Repositories.Implementations;
using FoodDeliveryAPI.Repositories.Interfaces;
using FoodDeliveryAPI.Services.Implementations;
using FoodDeliveryAPI.Services.Interfaces;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext - default to PostgreSQL, optional in-memory fallback for local dev
var rawDatabaseUrl = builder.Configuration["DATABASE_URL"];
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
var useInMemoryDatabase = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");

var postgresConnectionString =
    !string.IsNullOrWhiteSpace(rawDatabaseUrl)
        ? ConvertDatabaseUrlToConnectionString(rawDatabaseUrl)
        : defaultConnection;

if (!useInMemoryDatabase && string.IsNullOrWhiteSpace(postgresConnectionString))
{
    throw new InvalidOperationException(
        "PostgreSQL connection string is missing. Configure DATABASE_URL or ConnectionStrings:DefaultConnection.");
}

builder.Services.AddDbContext<FoodDeliveryDbContext>(options =>
{
    if (useInMemoryDatabase)
    {
        options.UseInMemoryDatabase("FoodDeliveryDb");
    }
    else
    {
        options.UseNpgsql(postgresConnectionString!);
    }
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"] ?? "ThisIsADevelopmentOnlySuperSecretJwtKey123!";
        var issuer = builder.Configuration["Jwt:Issuer"] ?? "FoodDeliveryAPI";
        var audience = builder.Configuration["Jwt:Audience"] ?? "FoodDeliveryAPI.Client";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpClient();

// Repository registrations
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddScoped<IFoodRepository, FoodRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

// Service registrations
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IFoodService, FoodService>();
builder.Services.AddScoped<IFoodImageService, FoodImageService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        var allowedOrigins = builder.Configuration["CORS__ALLOWED_ORIGINS"];

        if (!string.IsNullOrWhiteSpace(allowedOrigins))
        {
            var origins = allowedOrigins
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            policy.WithOrigins(origins)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

var app = builder.Build();

var renderPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(renderPort))
{
    app.Urls.Add($"http://0.0.0.0:{renderPort}");
}

try
{
    // Configure middleware
    if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("EnableSwagger"))
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "ziggy-api" }));
    app.MapControllers();

    // Create database and seed data
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<FoodDeliveryDbContext>();
        context.Database.EnsureCreated();

        if (!context.Users.Any())
        {
            context.Users.AddRange(
                new User
                {
                    Name = "Platform Admin",
                    Email = "admin@ziggy.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = "Admin"
                },
                new User
                {
                    Name = "Rahul Customer",
                    Email = "customer@ziggy.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer@123"),
                    Role = "Customer"
                },
                new User
                {
                    Name = "Aman Rider",
                    Email = "delivery@ziggy.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Delivery@123"),
                    Role = "DeliveryAgent"
                }
            );
            context.SaveChanges();
        }

        if (!context.FoodCategories.Any())
        {
            context.FoodCategories.AddRange(
                new FoodCategory { Name = "Main Course" },
                new FoodCategory { Name = "Desserts" },
                new FoodCategory { Name = "Starters" },
                new FoodCategory { Name = "Beverages" }
            );
            context.SaveChanges();
        }

        if (!context.Restaurants.Any())
        {
            SeedRestaurants(context);
        }

        if (context.Restaurants.Count() < 25)
        {
            SeedRestaurants(context);
        }

        var dishImageRules = GetDishImageRules(app.Configuration);

        if (context.Foods.Count() < 250)
        {
            SeedFoods(context, 250, dishImageRules);
        }

        // Ensure all existing dishes have unique, dish-specific image URLs.
        var foodsToRefresh = context.Foods
            .Include(f => f.Restaurant)
            .ToList();

        var refreshed = false;
        foreach (var food in foodsToRefresh)
        {
            var baseDishName = food.Name.Contains(" - ", StringComparison.Ordinal)
                ? food.Name.Split(" - ", StringSplitOptions.RemoveEmptyEntries)[0]
                : food.Name;

            var expected = BuildDishImageUrl(baseDishName, food.Restaurant?.Name ?? "Restaurant", food.RestaurantId, food.Id % 10, 0, dishImageRules);
            if (food.ImageUrl != expected)
            {
                food.ImageUrl = expected;
                refreshed = true;
            }
        }

        if (refreshed)
        {
            context.SaveChanges();
        }
    }

    Console.WriteLine("FoodDeliveryAPI started successfully.");

    await app.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"FATAL ERROR: {ex}");
    throw;
}

static string ConvertDatabaseUrlToConnectionString(string databaseUrl)
{
    // Render-style value: postgres://user:password@host:5432/database
    if (!Uri.TryCreate(databaseUrl, UriKind.Absolute, out var uri))
    {
        return databaseUrl;
    }

    var userInfo = uri.UserInfo.Split(':', 2);
    var username = Uri.UnescapeDataString(userInfo[0]);
    var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
    var database = uri.AbsolutePath.Trim('/');

    var builder = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port > 0 ? uri.Port : 5432,
        Username = username,
        Password = password,
        Database = database,
        SslMode = SslMode.Require
    };

    return builder.ConnectionString;
}

static void SeedRestaurants(FoodDeliveryDbContext context)
{
    var catalog = new List<(string Name, string Description, string Location, decimal Rating, string ImageUrl)>
    {
        ("Spice Route", "North Indian favorites, biryanis, and tandoor specials.", "Noida Sector 18", 4.4m, "https://images.unsplash.com/photo-1552566626-52f8b828add9?q=80&w=1200"),
        ("Coastal Bowl", "South Indian and coastal comfort food.", "Gurgaon CyberHub", 4.3m, "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?q=80&w=1200"),
        ("Tandoor Tales", "Signature kebabs, curries, and handcrafted breads.", "Delhi Connaught Place", 4.5m, "https://images.unsplash.com/photo-1466978913421-dad2ebd01d17?q=80&w=1200"),
        ("Urban Biryani House", "Layered biryanis and mughlai delicacies.", "Noida Sector 62", 4.2m, "https://images.unsplash.com/photo-1481833761820-0509d3217039?q=80&w=1200"),
        ("Wok & Flame", "Asian stir-fries, noodles, and rice bowls.", "Gurgaon Sector 29", 4.1m, "https://images.unsplash.com/photo-1578474846511-04ba529f0b88?q=80&w=1200"),
        ("Curry Junction", "Homestyle curries and thali meals.", "Ghaziabad Indirapuram", 4.0m, "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?q=80&w=1200"),
        ("Nawabi Kitchen", "Rich Awadhi gravies and kebab platters.", "Lucknow Hazratganj", 4.6m, "https://images.unsplash.com/photo-1514933651103-005eec06c04b?q=80&w=1200"),
        ("Punjab Express", "Butter-loaded Punjabi classics and combos.", "Chandigarh Sector 17", 4.2m, "https://images.unsplash.com/photo-1528605248644-14dd04022da1?q=80&w=1200"),
        ("Dosa District", "Crispy dosas, idli varieties, and filter coffee.", "Bengaluru Indiranagar", 4.3m, "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?q=80&w=1200"),
        ("Roll Republic", "Kathi rolls, wraps, and quick bites.", "Kolkata Park Street", 4.1m, "https://images.unsplash.com/photo-1559339352-11d035aa65de?q=80&w=1200"),
        ("Bombay Street Co.", "Mumbai street food and chaats.", "Mumbai Bandra", 4.3m, "https://images.unsplash.com/photo-1424847651672-bf20a4b0982b?q=80&w=1200"),
        ("The Kebab Club", "Grilled meats and smoky starters.", "Delhi Rajouri Garden", 4.4m, "https://images.unsplash.com/photo-1550966871-3ed3cdb5ed0c?q=80&w=1200"),
        ("Bowl Theory", "Healthy bowls and protein-rich plates.", "Pune Hinjewadi", 4.0m, "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?q=80&w=1200"),
        ("Royal Rasoi", "Festive Indian meals and family packs.", "Jaipur C-Scheme", 4.5m, "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?q=80&w=1200"),
        ("Pasta & Peppers", "Italian pasta, pizza, and sides.", "Hyderabad Jubilee Hills", 4.1m, "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?q=80&w=1200"),
        ("Sushi Saga", "Fresh sushi rolls and Japanese bowls.", "Bengaluru Koramangala", 4.4m, "https://images.unsplash.com/photo-1579871494447-9811cf80d66c?q=80&w=1200"),
        ("Mexi Fiesta", "Tacos, burritos, and loaded nachos.", "Pune Kalyani Nagar", 4.0m, "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?q=80&w=1200"),
        ("Burger Borough", "Smash burgers, fries, and shakes.", "Delhi Saket", 4.2m, "https://images.unsplash.com/photo-1559339352-11d035aa65de?q=80&w=1200"),
        ("Pizza Planet", "Wood-fired pizzas and garlic breads.", "Noida Sector 75", 4.1m, "https://images.unsplash.com/photo-1466978913421-dad2ebd01d17?q=80&w=1200"),
        ("Green Leaf Meals", "Vegan and salad-friendly wholesome meals.", "Gurgaon Golf Course Road", 4.0m, "https://images.unsplash.com/photo-1424847651672-bf20a4b0982b?q=80&w=1200"),
        ("Chai & Snacks Lab", "Tea-time snacks and baked munchies.", "Ahmedabad CG Road", 4.1m, "https://images.unsplash.com/photo-1528605248644-14dd04022da1?q=80&w=1200"),
        ("Ramen Republic", "Ramen bowls and Japanese comfort food.", "Chennai Nungambakkam", 4.3m, "https://images.unsplash.com/photo-1481833761820-0509d3217039?q=80&w=1200"),
        ("Grill Garden", "BBQ platters and grilled signature dishes.", "Indore Vijay Nagar", 4.2m, "https://images.unsplash.com/photo-1550966871-3ed3cdb5ed0c?q=80&w=1200"),
        ("Dessert Den", "Cakes, brownies, and chilled desserts.", "Kolkata Salt Lake", 4.4m, "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?q=80&w=1200"),
        ("Midnight Munchies", "Late-night combos and comfort food.", "Mumbai Andheri", 4.0m, "https://images.unsplash.com/photo-1514933651103-005eec06c04b?q=80&w=1200")
    };

    var existingNames = context.Restaurants.Select(r => r.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
    var missingRestaurants = catalog
        .Where(r => !existingNames.Contains(r.Name))
        .Select(r => new Restaurant
        {
            Name = r.Name,
            Description = r.Description,
            Location = r.Location,
            Rating = r.Rating,
            ImageUrl = r.ImageUrl,
            CreatedAt = DateTime.UtcNow
        })
        .ToList();

    if (missingRestaurants.Any())
    {
        context.Restaurants.AddRange(missingRestaurants);
        context.SaveChanges();
    }
}

static void SeedFoods(FoodDeliveryDbContext context, int targetFoodCount, IReadOnlyList<(string Keyword, string Url)> dishImageRules)
{
    var restaurants = context.Restaurants.OrderBy(r => r.Id).ToList();
    if (!restaurants.Any()) return;

    var categoryIds = context.FoodCategories.ToDictionary(c => c.Name, c => c.Id);
    var startersId = categoryIds["Starters"];
    var mainCourseId = categoryIds["Main Course"];
    var dessertsId = categoryIds["Desserts"];
    var beveragesId = categoryIds["Beverages"];

    var starters = new[]
    {
        "Paneer Tikka", "Crispy Corn", "Veg Spring Roll", "Chicken 65", "Hara Bhara Kebab",
        "Peri Peri Fries", "Chilli Paneer", "Honey Chilli Potato", "Tandoori Wings", "Stuffed Mushrooms"
    };

    var mains = new[]
    {
        "Butter Chicken", "Kadai Paneer", "Dal Makhani", "Chicken Biryani", "Veg Biryani",
        "Mutton Rogan Josh", "Prawn Curry", "Thai Green Curry", "Veg Alfredo Pasta", "Paneer Butter Masala",
        "Fish Tikka Masala", "Hyderabadi Dum Biryani", "Chole Bhature Combo", "Rajma Chawal Bowl", "Schezwan Noodles"
    };

    var desserts = new[]
    {
        "Gulab Jamun", "Brownie Sundae", "Rasmalai", "Chocolate Mousse", "Kulfi Falooda",
        "Cheesecake Slice", "Shahi Tukda", "Tiramisu Cup"
    };

    var beverages = new[]
    {
        "Masala Chaas", "Lemon Iced Tea", "Cold Coffee", "Mango Shake", "Fresh Lime Soda",
        "Filter Coffee", "Mint Mojito", "Hot Chocolate"
    };

    var existingKeys = context.Foods.Select(f => $"{f.RestaurantId}:{f.Name}").ToHashSet(StringComparer.OrdinalIgnoreCase);
    var newFoods = new List<Food>();
    var currentCount = context.Foods.Count();
    var cycle = 0;

    while (currentCount + newFoods.Count < targetFoodCount)
    {
        foreach (var restaurant in restaurants)
        {
            for (var slot = 0; slot < 10 && currentCount + newFoods.Count < targetFoodCount; slot++)
            {
                var (categoryId, baseName, imageUrl, basePrice) = slot switch
                {
                    0 or 1 or 2 =>
                        (startersId,
                        starters[(restaurant.Id + slot + cycle) % starters.Length],
                        string.Empty,
                        149m),
                    3 or 4 or 5 or 6 =>
                        (mainCourseId,
                        mains[(restaurant.Id + slot + cycle) % mains.Length],
                        string.Empty,
                        239m),
                    7 or 8 =>
                        (dessertsId,
                        desserts[(restaurant.Id + slot + cycle) % desserts.Length],
                        string.Empty,
                        99m),
                    _ =>
                        (beveragesId,
                        beverages[(restaurant.Id + slot + cycle) % beverages.Length],
                        string.Empty,
                        79m)
                };

                var name = $"{baseName} - {restaurant.Name}";
                var key = $"{restaurant.Id}:{name}";
                if (existingKeys.Contains(key))
                {
                    continue;
                }

                var price = basePrice + ((restaurant.Id + slot + cycle) % 12) * 10;
                var dishImageUrl = BuildDishImageUrl(baseName, restaurant.Name, restaurant.Id, slot, cycle, dishImageRules);
                newFoods.Add(new Food
                {
                    Name = name,
                    Description = $"{baseName} crafted fresh by {restaurant.Name}.",
                    Price = price,
                    CategoryId = categoryId,
                    RestaurantId = restaurant.Id,
                    ImageUrl = dishImageUrl,
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                });

                existingKeys.Add(key);
            }
        }

        cycle++;
        if (cycle > 20) break;
    }

    if (newFoods.Any())
    {
        context.Foods.AddRange(newFoods);
        context.SaveChanges();
    }
}

static string BuildDishImageUrl(string dishName, string restaurantName, int restaurantId, int slot, int cycle, IReadOnlyList<(string Keyword, string Url)> dishImageRules)
{
    var name = dishName.Trim().ToLowerInvariant();

    foreach (var (keyword, url) in dishImageRules)
    {
        if (name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
        {
            return url;
        }
    }

    // Use stable, curated URLs by dish keyword to avoid repeated results from source.unsplash.
    var image = name switch
    {
        var n when n.Contains("biryani") => "https://images.unsplash.com/photo-1563379091339-03246963d96c?auto=format&fit=crop&w=1200&q=80",
        var n when n.Contains("butter chicken") => "https://images.unsplash.com/photo-1603894584373-5ac82b2ae398?auto=format&fit=crop&w=1200&q=80",
        var n when n.Contains("paneer") => "https://images.unsplash.com/photo-1631452180519-c014fe946bc7?auto=format&fit=crop&w=1200&q=80",
        var n when n.Contains("dal") => "https://images.unsplash.com/photo-1546833999-b9f581a1996d?auto=format&fit=crop&w=1200&q=80",
        var n when n.Contains("mutton") || n.Contains("rogan") => "https://images.unsplash.com/photo-1544025162-d76694265947?auto=format&fit=crop&w=1200&q=80",
        var n when n.Contains("prawn") || n.Contains("fish") => "https://images.unsplash.com/photo-1611270629569-8b357cb88da9?auto=format&fit=crop&w=1200&q=80",
        var n when n.Contains("noodles") => "https://images.unsplash.com/photo-1617622141675-d3005b9067c5?auto=format&fit=crop&w=1200&q=80",
        var n when n.Contains("pasta") => "https://images.unsplash.com/photo-1621996346565-e3dbc646d9a9?auto=format&fit=crop&w=1200&q=80",
        var n when n.Contains("spring roll") => "https://images.unsplash.com/photo-1601050690597-df0568f70950?auto=format&fit=crop&w=1200&q=80",
        var n when n.Contains("fries") => "https://images.unsplash.com/photo-1573080496219-bb080dd4f877?auto=format&fit=crop&w=1200&q=80",
        var n when n.Contains("wings") => "https://images.unsplash.com/photo-1527477396000-e27163b481c2?auto=format&fit=crop&w=1200&q=80",
        var n when n.Contains("mushroom") => "https://images.unsplash.com/photo-1504674900247-0877df9cc836?auto=format&fit=crop&w=1200&q=80",
        var n when n.Contains("gulab") || n.Contains("rasmalai") || n.Contains("kulfi") || n.Contains("shahi") => "https://images.unsplash.com/photo-1605197161470-5c1f7b1dbf5f?auto=format&fit=crop&w=1200&q=80",
        var n when n.Contains("brownie") || n.Contains("chocolate") || n.Contains("mousse") => "https://images.unsplash.com/photo-1564355808539-22fda35bed7e?auto=format&fit=crop&w=1200&q=80",
        var n when n.Contains("cheesecake") || n.Contains("tiramisu") => "https://images.unsplash.com/photo-1533134242443-d4fd215305ad?auto=format&fit=crop&w=1200&q=80",
        var n when n.Contains("coffee") => "https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?auto=format&fit=crop&w=1200&q=80",
        var n when n.Contains("tea") || n.Contains("chaas") || n.Contains("mojito") || n.Contains("soda") => "https://images.unsplash.com/photo-1461823385004-d7660947a7c0?auto=format&fit=crop&w=1200&q=80",
        var n when n.Contains("shake") => "https://images.unsplash.com/photo-1577805947697-89e18249d767?auto=format&fit=crop&w=1200&q=80",
        _ => "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=1200&q=80"
    };

    return image;
}

static List<(string Keyword, string Url)> GetDishImageRules(IConfiguration configuration)
{
    var rules = configuration
        .GetSection("FoodImage:SeedImageMap")
        .GetChildren()
        .Select(c => (Keyword: c.Key.Trim().ToLowerInvariant(), Url: (c.Value ?? string.Empty).Trim()))
        .Where(x => !string.IsNullOrWhiteSpace(x.Keyword) && Uri.IsWellFormedUriString(x.Url, UriKind.Absolute))
        .ToList();

    return rules;
}
