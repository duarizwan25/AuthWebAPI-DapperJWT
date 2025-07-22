using AuthWebAPI.DTOs;
using AuthWebAPI.Interfaces;
using AuthWebAPI.Models;
using Dapper;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthWebAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IDbConnection _db;
        private readonly IConfiguration _config;

        public AuthService(IDbConnection db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<string> RegisterAsync(UserDto request)
        {
            var existingUser = await _db.QueryFirstOrDefaultAsync<UserModel>(
                "SELECT * FROM Users WHERE Username = @Username", new { request.Username });

            if (existingUser != null)
                return "User already exists.";

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            string sql = "INSERT INTO Users (Id, Username, PasswordHash, Email, Roles) VALUES (@Id, @Username, @PasswordHash, @Email, @Roles)";
            var newUser = new UserModel
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                PasswordHash = passwordHash,
                Email = request.Username + "@example.com",
                Roles = "User"
            };

            await _db.ExecuteAsync(sql, newUser);
            return "User created successfully.";
        }

        public async Task<string> LoginAsync(UserDto request)
        {
            var user = await _db.QueryFirstOrDefaultAsync<UserModel>(
                "SELECT * FROM Users WHERE Username = @Username", new { request.Username });

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return "Username/password is wrong";

            return CreateToken(user);
        }

        private string CreateToken(UserModel user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Roles)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["AppSettings:Token"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: _config["AppSettings:Issuer"],
                audience: _config["AppSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
