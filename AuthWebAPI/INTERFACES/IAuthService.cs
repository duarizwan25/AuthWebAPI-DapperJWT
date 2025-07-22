using AuthWebAPI.DTOs;
using AuthWebAPI.Models;

namespace AuthWebAPI.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(UserDto request);
        Task<string> LoginAsync(UserDto request);
    }
}
