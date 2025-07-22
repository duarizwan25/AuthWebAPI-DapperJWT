using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using AuthWebAPI.Models;
using System.Data.SqlClient;
using Dapper;

namespace AuthWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly string _connectionString;

        public UserController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MyDB");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
        {
            using var connection = new SqlConnection(_connectionString);
            var users = await connection.QueryAsync<UserModel>("SELECT Id, Username, Email FROM Users");
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUser(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            var user = await connection.QuerySingleOrDefaultAsync<UserModel>(
                "SELECT Id, Username, Email FROM Users WHERE Id = @Id", new { Id = id });

            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserModel>> CreateUser(UserModel userModel)
        {
            userModel.Id = Guid.NewGuid();

            using var connection = new SqlConnection(_connectionString);
            var sql = "INSERT INTO Users (Id, Username, Email) VALUES (@Id, @Username, @Email)";
            await connection.ExecuteAsync(sql, userModel);

            return CreatedAtAction(nameof(GetUser), new { id = userModel.Id }, userModel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UserModel userModel)
        {
            if (id != userModel.Id) return BadRequest();

            using var connection = new SqlConnection(_connectionString);
            var sql = "UPDATE Users SET Username = @Username, Email = @Email WHERE Id = @Id";
            var affected = await connection.ExecuteAsync(sql, userModel);

            if (affected == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "DELETE FROM Users WHERE Id = @Id";
            var affected = await connection.ExecuteAsync(sql, new { Id = id });

            if (affected == 0) return NotFound();
            return NoContent();
        }
    }
}
