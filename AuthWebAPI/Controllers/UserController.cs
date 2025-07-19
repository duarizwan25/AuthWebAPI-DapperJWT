using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthWebAPI.Data;
using EntityUser = AuthWebAPI.Entities.User;
using AuthWebAPI.Models;
using AuthWebAPI.Entities;
using Microsoft.AspNetCore.Authorization;


namespace AuthWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _context;

        public UserController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            var result = users.Select(u => new UserModel
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var userModel = new UserModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            return Ok(userModel);
        }

        [HttpPost]
        public async Task<ActionResult<UserModel>> CreateUser(UserModel userModel)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = userModel.Username,
                Email = userModel.Email
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            userModel.Id = user.Id;
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userModel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UserModel userModel)
        {
            if (id != userModel.Id) return BadRequest();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Username = userModel.Username;
            user.Email = userModel.Email;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
