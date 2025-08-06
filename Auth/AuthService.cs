using GoBest.Auth.DTO;
using GoBest.Data;
using GoBest.Exceptions;
using GoBest.Models;
using GoBest.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GoBest.Auth
{
    public class AuthService
    {
        private readonly AuthRepository _authRepository;
        private readonly IConfiguration _config;

        public AuthService(AuthRepository authRepository, IConfiguration config)
        {
            _authRepository = authRepository;
            _config = config;
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("admin123");
            Console.WriteLine(hashedPassword);
        }

        public async Task<AuthResponse> RegisterAsync([FromBody] RegisterRequest request)
        {
            if (await _authRepository.UserExists(request.Email))
                throw BusinessException.EmailAlreadyExists();

            var user = AuthMapper.ToUser(request);

            await _authRepository.CreateUser(user);

            return AuthMapper.ToResponse(user, GenerateJwtToken(user));
        }

        public async Task<AuthResponse> LoginAsync(string email, string password)
        {
            var user = await _authRepository.GetUserByEmail(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw BusinessException.InvalidCredentials();

            return AuthMapper.ToResponse(user, GenerateJwtToken(user));
        }


        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
