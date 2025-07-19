using AuthWebAPI.Entities;
using AuthWebAPI.Model;
using AuthWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService service;

        public AuthController(IAuthService service)
        {
            this.service = service;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User?>> Register(UserDto request)
        {
            var user = await service.RegisterAsync(request);
            if (user is null)
                return BadRequest("Username already exists!");
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var token = await service.LoginAsync(request);
            if (token is null)
                return BadRequest("Username/password is wrong");
            return Ok(token);
        }
        [HttpGet("Auth-endpoint")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public ActionResult AuthCheck()
        {
            return Ok();
        }

        [HttpGet("Admin-endpoint")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles ="Admin")]
        public ActionResult AdminCheck()
        {
            return Ok();
        }
    }
}
