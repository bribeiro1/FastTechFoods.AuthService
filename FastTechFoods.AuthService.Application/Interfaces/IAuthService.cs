using FastTechFoods.AuthService.Application.DTOs;

namespace FastTechFoods.AuthService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> AuthenticateAsync(LoginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
    }
}