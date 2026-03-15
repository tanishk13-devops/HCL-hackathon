using Microsoft.EntityFrameworkCore;
using FoodDeliveryAPI.Data;
using FoodDeliveryAPI.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
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

// Add DbContext - Use PostgreSQL
var rawDatabaseUrl = builder.Configuration["DATABASE_URL"];
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");

var postgresConnectionString =
    !string.IsNullOrWhiteSpace(rawDatabaseUrl)
        ? ConvertDatabaseUrlToConnectionString(rawDatabaseUrl)
        : defaultConnection;

if (string.IsNullOrWhiteSpace(postgresConnectionString))
{
    throw new InvalidOperationException(
        "PostgreSQL connection string is missing. Configure DATABASE_URL or ConnectionStrings:DefaultConnection.");
}

builder.Services.AddDbContext<FoodDeliveryDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));

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
    app.UseAuthorization();
    app.MapControllers();

    // Create database and seed data
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<FoodDeliveryDbContext>();
        context.Database.EnsureCreated();

        // Add sample foods if none exist
        if (!context.Foods.Any())
        {
            var foods = new[]
            {
                new Food { Name = "Biryani", Description = "Fragrant basmati rice cooked with aromatic spices", Price = 250, Category = "Rice Dishes", ImageUrl = "https://images.pexels.com/photos/12737656/pexels-photo-12737656.jpeg?auto=compress&cs=tinysrgb&w=1200" },
                new Food { Name = "Butter Chicken", Description = "Tender chicken simmered in creamy tomato gravy", Price = 350, Category = "Curries", ImageUrl = "https://images.pexels.com/photos/7625056/pexels-photo-7625056.jpeg?auto=compress&cs=tinysrgb&w=1200" },
                new Food { Name = "Paneer Tikka", Description = "Char-grilled paneer with smoky spices", Price = 200, Category = "Appetizers", ImageUrl = "https://images.unsplash.com/photo-1599487488170-d11ec9c172f0?q=80&w=1200" },
                new Food { Name = "Tandoori Chicken", Description = "Classic tandoor roasted chicken with herbs", Price = 300, Category = "Grilled", ImageUrl = "https://images.pexels.com/photos/616354/pexels-photo-616354.jpeg?auto=compress&cs=tinysrgb&w=1200" },
                new Food { Name = "Garlic Naan", Description = "Oven baked naan brushed with garlic butter", Price = 50, Category = "Breads", ImageUrl = "https://images.pexels.com/photos/9797029/pexels-photo-9797029.jpeg?auto=compress&cs=tinysrgb&w=1200" },
                new Food { Name = "Samosa", Description = "Golden pastry filled with spiced potatoes", Price = 30, Category = "Appetizers", ImageUrl = "https://images.unsplash.com/photo-1601050690117-94f5f6fa5f30?q=80&w=1200" },
                new Food { Name = "Dal Makhani", Description = "Slow-cooked lentils finished with cream", Price = 150, Category = "Curries", ImageUrl = "https://images.unsplash.com/photo-1546833999-b9f581a1996d?q=80&w=1200" },
                new Food { Name = "Chole Bhature", Description = "Spiced chickpeas served with fluffy bhature", Price = 120, Category = "Street Food", ImageUrl = "https://images.unsplash.com/photo-1626082927389-6cd097cdc6ec?q=80&w=1200" },
                new Food { Name = "Shahi Tukda", Description = "Royal bread pudding with saffron & nuts", Price = 80, Category = "Desserts", ImageUrl = "https://images.unsplash.com/photo-1631452180519-c014fe946bc7?q=80&w=1200" },
                new Food { Name = "Gulab Jamun", Description = "Soft milk dumplings soaked in rose syrup", Price = 60, Category = "Desserts", ImageUrl = "https://images.unsplash.com/photo-1666190092159-3171cf0fbb12?q=80&w=1200" }
            };

            context.Foods.AddRange(foods);
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
