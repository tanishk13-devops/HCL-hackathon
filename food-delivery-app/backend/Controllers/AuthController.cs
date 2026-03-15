using Microsoft.AspNetCore.Mvc;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Data;
using System.Security.Cryptography;
using System.Text;

namespace FoodDeliveryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly FoodDeliveryDbContext _context;

        public AuthController(FoodDeliveryDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and password are required");

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == normalizedEmail);
            if (existingUser != null)
                return BadRequest("Email already registered");

            var user = new User
            {
                Email = normalizedEmail,
                Name = request.Name.Trim(),
                Phone = request.Phone?.Trim(),
                PasswordHash = HashPassword(request.Password),
                Role = "Customer"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { id = user.Id, email = user.Email, name = user.Name, role = user.Role });
        }

        [HttpPost("login")]
        public ActionResult<User> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and password are required");

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = _context.Users.FirstOrDefault(u => u.Email == normalizedEmail);
            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            return Ok(new { id = user.Id, email = user.Email, name = user.Name, role = user.Role });
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hash);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput.Equals(hash);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
