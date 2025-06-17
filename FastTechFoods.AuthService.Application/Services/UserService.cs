using FastTechFoods.AuthService.Application.DTOs;
using FastTechFoods.AuthService.Application.Interfaces;
using FastTechFoods.AuthService.Domain.Constants;
using FastTechFoods.AuthService.Domain.Entities;
using FastTechFoods.AuthService.Domain.Interfaces;

namespace FastTechFoods.AuthService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly ITokenService _token;
        public UserService(IUserRepository repo, ITokenService token)
        {
            _repo = repo;
            _token = token;
        }
        public async Task<AuthResponse> AuthenticateAsync(LoginRequest request)
        {
            var user = await _repo.GetByEmailOrCpfAsync(request.Login);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return AuthResponse.Fail("Credenciais inválidas");
            return AuthResponse.Success(user, _token.GenerateToken(user));
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (!UserRoles.Todos.Contains(request.Role))
                return AuthResponse.Fail("Role inválida");

            if (await _repo.ExistsAsync(request.Email, request.Cpf))
                return AuthResponse.Fail("Usuário já existe");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Cpf = request.Cpf,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role
            };

            await _repo.AddAsync(user);
            return AuthResponse.Success(user, _token.GenerateToken(user));
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _repo.GetAllAsync();
            return users.Select(u => new UserDto(u));
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string role)
        {
            var users = await _repo.GetByRoleAsync(role);
            return users.Select(u => new UserDto(u));
        }
    }
}