using FastTechFoods.AuthService.Domain.Entities;
using FastTechFoods.AuthService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastTechFoods.AuthService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _context;
        public UserRepository(AuthDbContext context) => _context = context;

        public Task<User> GetByEmailOrCpfAsync(string login) =>
            _context.Users.FirstOrDefaultAsync(u => u.Email == login || u.Cpf == login);

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsAsync(string email, string cpf) =>
            _context.Users.AnyAsync(u => u.Email == email || u.Cpf == cpf);
    }
}
