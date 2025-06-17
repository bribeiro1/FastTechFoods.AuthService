using FastTechFoods.AuthService.Application.DTOs;
using FastTechFoods.AuthService.Domain.Entities;

namespace FastTechFoods.AuthService.Application.Interfaces
{
    public interface IUserService
    {
        Task<AuthResponse> AuthenticateAsync(LoginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string role);
    }
}