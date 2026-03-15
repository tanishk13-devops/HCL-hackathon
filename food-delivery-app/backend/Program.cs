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
            context.Restaurants.AddRange(
                new Restaurant
                {
                    Name = "Spice Route",
                    Description = "North Indian favorites, biryanis, and tandoor specials.",
                    Location = "Noida Sector 18",
                    Rating = 4.4m,
                    ImageUrl = "https://images.unsplash.com/photo-1552566626-52f8b828add9?q=80&w=1200"
                },
                new Restaurant
                {
                    Name = "Coastal Bowl",
                    Description = "South Indian and coastal comfort food.",
                    Location = "Gurgaon CyberHub",
                    Rating = 4.3m,
                    ImageUrl = "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?q=80&w=1200"
                }
            );
            context.SaveChanges();
        }

        if (!context.Foods.Any())
        {
            var mainCourseCategoryId = context.FoodCategories.First(c => c.Name == "Main Course").Id;
            var dessertsCategoryId = context.FoodCategories.First(c => c.Name == "Desserts").Id;
            var startersCategoryId = context.FoodCategories.First(c => c.Name == "Starters").Id;

            var spiceRouteId = context.Restaurants.First(r => r.Name == "Spice Route").Id;
            var coastalBowlId = context.Restaurants.First(r => r.Name == "Coastal Bowl").Id;

            context.Foods.AddRange(
                new Food
                {
                    Name = "Butter Chicken",
                    Description = "Classic creamy tomato gravy with tender chicken.",
                    Price = 349,
                    CategoryId = mainCourseCategoryId,
                    RestaurantId = spiceRouteId,
                    ImageUrl = "https://images.pexels.com/photos/7625056/pexels-photo-7625056.jpeg?auto=compress&cs=tinysrgb&w=1200",
                    IsAvailable = true
                },
                new Food
                {
                    Name = "Paneer Tikka",
                    Description = "Tandoor roasted paneer cubes and peppers.",
                    Price = 229,
                    CategoryId = startersCategoryId,
                    RestaurantId = spiceRouteId,
                    ImageUrl = "https://images.unsplash.com/photo-1599487488170-d11ec9c172f0?q=80&w=1200",
                    IsAvailable = true
                },
                new Food
                {
                    Name = "Prawn Curry",
                    Description = "Coconut-based spicy prawn curry.",
                    Price = 399,
                    CategoryId = mainCourseCategoryId,
                    RestaurantId = coastalBowlId,
                    ImageUrl = "https://images.unsplash.com/photo-1604908176997-4315f57d89b4?q=80&w=1200",
                    IsAvailable = true
                },
                new Food
                {
                    Name = "Gulab Jamun",
                    Description = "Soft milk dumplings in rose sugar syrup.",
                    Price = 89,
                    CategoryId = dessertsCategoryId,
                    RestaurantId = coastalBowlId,
                    ImageUrl = "https://images.unsplash.com/photo-1666190092159-3171cf0fbb12?q=80&w=1200",
                    IsAvailable = true
                }
            );
            context.SaveChanges();
        }
    }

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
