using FoodDeliveryAPI.DTOs;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Repositories.Interfaces;
using FoodDeliveryAPI.Services.Interfaces;
using System.Security.Cryptography;

namespace FoodDeliveryAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            User? exists;
            try
            {
                exists = await _userRepository.GetByEmailAsync(email);
            }
            catch
            {
                throw new InvalidOperationException("Authentication service unavailable. Please try again.");
            }

            if (exists != null)
            {
                throw new InvalidOperationException("Email already registered.");
            }

            var role = NormalizeRole(request.Role);
            var user = new User
            {
                Name = request.Name.Trim(),
                Email = email,
                Phone = request.Phone?.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = role
            };

            try
            {
                await _userRepository.AddAsync(user);
            }
            catch
            {
                throw new InvalidOperationException("Authentication service unavailable. Please try again.");
            }

            return new AuthResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Token = _tokenService.CreateToken(user)
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            User? user;
            try
            {
                user = await _userRepository.GetByEmailAsync(email);
            }
            catch
            {
                throw new InvalidOperationException("Authentication service unavailable. Please try again.");
            }

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            return new AuthResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Token = _tokenService.CreateToken(user)
            };
        }

        private static string NormalizeRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return "Customer";

            return role.Trim() switch
            {
                "Admin" => "Admin",
                "DeliveryAgent" => "DeliveryAgent",
                _ => "Customer"
            };
        }
    }
}
