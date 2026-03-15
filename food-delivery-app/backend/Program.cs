using Microsoft.EntityFrameworkCore;
using FoodDeliveryAPI.Data;
using FoodDeliveryAPI.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext - Use InMemory for development
builder.Services.AddDbContext<FoodDeliveryDbContext>(options =>
    options.UseInMemoryDatabase("FoodDeliveryDB"));

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
                new Food { Name = "Paneer Tikka", Description = "Char-grilled paneer with smoky spices", Price = 200, Category = "Appetizers", ImageUrl = "https://images.pexels.com/photos/5410400/pexels-photo-5410400.jpeg?auto=compress&cs=tinysrgb&w=1200" },
                new Food { Name = "Tandoori Chicken", Description = "Classic tandoor roasted chicken with herbs", Price = 300, Category = "Grilled", ImageUrl = "https://images.pexels.com/photos/616354/pexels-photo-616354.jpeg?auto=compress&cs=tinysrgb&w=1200" },
                new Food { Name = "Garlic Naan", Description = "Oven baked naan brushed with garlic butter", Price = 50, Category = "Breads", ImageUrl = "https://images.pexels.com/photos/9797029/pexels-photo-9797029.jpeg?auto=compress&cs=tinysrgb&w=1200" },
                new Food { Name = "Samosa", Description = "Golden pastry filled with spiced potatoes", Price = 30, Category = "Appetizers", ImageUrl = "https://images.pexels.com/photos/2474661/pexels-photo-2474661.jpeg?auto=compress&cs=tinysrgb&w=1200" },
                new Food { Name = "Dal Makhani", Description = "Slow-cooked lentils finished with cream", Price = 150, Category = "Curries", ImageUrl = "https://images.pexels.com/photos/1640774/pexels-photo-1640774.jpeg?auto=compress&cs=tinysrgb&w=1200" },
                new Food { Name = "Chole Bhature", Description = "Spiced chickpeas served with fluffy bhature", Price = 120, Category = "Street Food", ImageUrl = "https://images.unsplash.com/photo-1626082927389-6cd097cdc6ec?q=80&w=1200" },
                new Food { Name = "Shahi Tukda", Description = "Royal bread pudding with saffron & nuts", Price = 80, Category = "Desserts", ImageUrl = "https://images.unsplash.com/photo-1601050690597-df0568f70950?q=80&w=1200" },
                new Food { Name = "Gulab Jamun", Description = "Soft milk dumplings soaked in rose syrup", Price = 60, Category = "Desserts", ImageUrl = "https://images.pexels.com/photos/11059539/pexels-photo-11059539.jpeg?auto=compress&cs=tinysrgb&w=1200" }
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
