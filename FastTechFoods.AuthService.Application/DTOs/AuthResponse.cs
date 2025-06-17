using FastTechFoods.AuthService.Domain.Entities;

namespace FastTechFoods.AuthService.Application.DTOs
{
    public class AuthResponse
    {
        public bool IsAuthenticated { get; set; }
        public string? Token { get; set; }
        public string? Role { get; set; }
        public string? Message { get; set; }

        public static AuthResponse Success(User user, string token) => new()
        {
            IsAuthenticated = true,
            Token = token,
            Role = user.Role,
            Message = null
        };

        public static AuthResponse Fail(string error) => new()
        {
            IsAuthenticated = false,
            Token = null,
            Role = null,
            Message = error
        };
    }
}
