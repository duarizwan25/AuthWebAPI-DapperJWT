using AuthWebAPI.DTOs;
using AuthWebAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AuthWebAPI.Models;
using System.Data.SqlClient;
using Dapper;
using BCrypt.Net;

namespace AuthWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto request)
        {
            var result = await _authService.RegisterAsync(request);
            if (result == "User already exists.")
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto request)
        {
            var result = await _authService.LoginAsync(request);
            if (result == "Username/password is wrong")
                return BadRequest(result);
            return Ok(result);
        }
    }
}
