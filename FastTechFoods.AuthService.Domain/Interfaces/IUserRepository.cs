using FastTechFoods.AuthService.Domain.Entities;

namespace FastTechFoods.AuthService.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByEmailOrCpfAsync(string login);
        Task AddAsync(User user);
        Task<bool> ExistsAsync(string email, string cpf);
    }
}