using GoBest.Models;
using GoBest.Users;

namespace GoBest.Auth.DTO
{
    public class AuthMapper
    {
        public static AuthResponse ToResponse(User user, string token)
        {
            return new AuthResponse
            {
                Token = token,
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }
        
        public static User ToUser(RegisterRequest request)
        {
            return new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = UserRole.Customer,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}