# Backend Implementation Guide

This document provides guidance for implementing the ASP.NET Core backend to work with the Angular frontend.

## Database Schema Overview

Based on the provided schema, implement the following entities:

### 1. Users Table
```sql
- UserId (PK, int, Identity)
- FirstName (nvarchar(100), NOT NULL)
- LastName (nvarchar(100), NOT NULL)
- Email (nvarchar(255), UNIQUE, NOT NULL)
- PasswordHash (nvarchar(255), NOT NULL)
- Role (nvarchar(50), NOT NULL) -- 'Admin' or 'Customer'
- CreatedAt (datetime, NOT NULL, DEFAULT GETDATE())
```

### 2. Books Table
```sql
- BookId (PK, int, Identity)
- Title (nvarchar(255), NOT NULL)
- Author (nvarchar(255), NOT NULL)
- Price (decimal(10,2), NOT NULL, CHECK Price >= 0)
- Description (nvarchar(MAX))
- StockQuantity (int, NOT NULL)
- CreatedAt (datetime, NOT NULL, DEFAULT GETDATE())
```

### 3. Carts Table
```sql
- CartId (PK, int, Identity)
- UserId (FK, int, NOT NULL) -- REFERENCES Users(UserId)
- CreatedAt (datetime, NOT NULL, DEFAULT GETDATE())
```

### 4. CartItems Table
```sql
- CartItemId (PK, int, Identity)
- CartId (FK, int, NOT NULL) -- REFERENCES Carts(CartId)
- BookId (FK, int, NOT NULL) -- REFERENCES Books(BookId)
- Quantity (int, NOT NULL)
```

### 5. Orders Table
```sql
- OrderId (PK, int, Identity)
- UserId (FK, int, NOT NULL) -- REFERENCES Users(UserId)
- TotalAmount (decimal(10,2), NOT NULL)
- Status (nvarchar(50), NOT NULL) -- 'Pending', 'Confirmed', 'Shipped', 'Delivered', 'Cancelled'
- OrderDate (datetime, NOT NULL, DEFAULT GETDATE())
```

### 6. OrderItems Table
```sql
- OrderItemId (PK, int, Identity)
- OrderId (FK, int, NOT NULL) -- REFERENCES Orders(OrderId)
- BookId (FK, int, NOT NULL) -- REFERENCES Books(BookId)
```

### 7. Reviews Table
```sql
- ReviewId (PK, int, Identity)
- UserId (FK, int, NOT NULL) -- REFERENCES Users(UserId)
- BookId (FK, int, NOT NULL) -- REFERENCES Books(BookId)
- Rating (int, NOT NULL, CHECK Rating BETWEEN 1 AND 5)
- Comment (nvarchar(MAX))
- CreatedAt (datetime, NOT NULL, DEFAULT GETDATE())
```

## Required NuGet Packages

```bash
# Entity Framework Core
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools

# JWT Authentication
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt

# Password Hashing
dotnet add package BCrypt.Net-Next

# CORS
# Already included in ASP.NET Core
```

## Startup Configuration (Program.cs)

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<BookstoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

## appsettings.json Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BookstoreDB;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyForJWTTokenGeneration123!",
    "Issuer": "BookstoreAPI",
    "Audience": "BookstoreAngularApp",
    "ExpirationMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## Sample Controller Structure

### AuthController.cs

```csharp
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // 1. Validate request
        // 2. Check if email already exists
        // 3. Hash password using BCrypt
        // 4. Create user in database
        // 5. Generate JWT token
        // 6. Return LoginResponse with token and user
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // 1. Validate request
        // 2. Find user by email
        // 3. Verify password using BCrypt
        // 4. Generate JWT token
        // 5. Return LoginResponse with token and user
    }
}
```

### BooksController.cs

```csharp
[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllBooks([FromQuery] string? search, 
                                                  [FromQuery] string? author,
                                                  [FromQuery] decimal? minPrice,
                                                  [FromQuery] decimal? maxPrice)
    {
        // Return filtered book list with average ratings
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(int id)
    {
        // Return book with average rating and review count
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateBook([FromBody] CreateBookRequest request)
    {
        // Create new book
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookRequest request)
    {
        // Update book
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        // Delete book
    }
}
```

### CartController.cs

```csharp
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        // Get current user's cart with items and book details
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        // Add item to cart or update quantity if exists
    }

    [HttpPut("items/{id}")]
    public async Task<IActionResult> UpdateCartItem(int id, [FromBody] UpdateCartItemRequest request)
    {
        // Update cart item quantity
    }

    [HttpDelete("items/{id}")]
    public async Task<IActionResult> RemoveCartItem(int id)
    {
        // Remove item from cart
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        // Remove all items from cart
    }
}
```

### OrdersController.cs

```csharp
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        // 1. Get cart items
        // 2. Calculate total
        // 3. Create order and order items
        // 4. Clear cart
        // 5. Update stock quantities
        // 6. Return order details
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders()
    {
        // Return current user's orders with items
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        // Return order details with items
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllOrders()
    {
        // Return all orders
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        // Update order status
    }
}
```

### ReviewsController.cs

```csharp
[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    [HttpGet("book/{bookId}")]
    public async Task<IActionResult> GetReviewsByBook(int bookId)
    {
        // Return all reviews for a book with user names
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request)
    {
        // Create review
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewRequest request)
    {
        // Update review (only if user owns it)
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(int id)
    {
        // Delete review (only if user owns it or is admin)
    }

    [HttpGet("my-reviews")]
    [Authorize]
    public async Task<IActionResult> GetMyReviews()
    {
        // Return current user's reviews
    }
}
```

## JWT Token Generation Helper

```csharp
public class JwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpirationMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

## Getting Current User in Controllers

```csharp
private int GetCurrentUserId()
{
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    return int.Parse(userIdClaim);
}

private string GetCurrentUserRole()
{
    return User.FindFirst(ClaimTypes.Role)?.Value;
}
```

## Important Implementation Notes

1. **Password Hashing**: Always use BCrypt.Net-Next for password hashing
   ```csharp
   var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
   var isValid = BCrypt.Net.BCrypt.Verify(password, passwordHash);
   ```

2. **Cart Management**: Create cart automatically when user registers or first adds item

3. **Order Creation**: 
   - Verify stock availability before creating order
   - Update stock quantities after order creation
   - Calculate total amount server-side (never trust client)

4. **Reviews**: 
   - Include user's full name when returning reviews
   - Calculate average rating when returning book details

5. **Error Handling**: Return appropriate HTTP status codes
   - 200: Success
   - 201: Created
   - 400: Bad Request
   - 401: Unauthorized
   - 403: Forbidden
   - 404: Not Found
   - 500: Internal Server Error

6. **Validation**: Use Data Annotations on DTOs
   ```csharp
   [Required]
   [EmailAddress]
   public string Email { get; set; }
   ```

## Database Migrations

```bash
# Create initial migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update

# Add new migration
dotnet ef migrations add AddReviewsTable

# Update database with specific migration
dotnet ef database update AddReviewsTable
```

## Testing Endpoints

Use tools like:
- **Swagger UI** (built-in)
- **Postman**
- **REST Client VS Code extension**

## Security Checklist

- ✅ Hash all passwords with BCrypt
- ✅ Use HTTPS in production
- ✅ Validate all inputs
- ✅ Use parameterized queries (EF Core does this)
- ✅ Implement proper authorization checks
- ✅ Add rate limiting (consider using AspNetCoreRateLimit)
- ✅ Enable CORS only for trusted origins
- ✅ Store JWT secret in environment variables in production
- ✅ Set appropriate token expiration times
- ✅ Implement refresh tokens for better security (optional)

---

This guide provides the foundation for implementing the backend. Adjust based on your specific requirements and best practices.
