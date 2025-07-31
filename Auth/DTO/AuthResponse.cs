using GoBest.Users;

namespace GoBest.Auth.DTO
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public long Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
    }

}