using GoBest.Models;

namespace GoBest.Auth.DTO
{
    public class AuthMapper {
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
    }
}