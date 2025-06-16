using FastTechFoods.AuthService.Domain.Entities;

namespace FastTechFoods.AuthService.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
