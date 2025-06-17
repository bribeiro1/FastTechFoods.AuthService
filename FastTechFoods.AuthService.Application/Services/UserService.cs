using FastTechFoods.AuthService.Application.DTOs;
using FastTechFoods.AuthService.Application.Interfaces;
using FastTechFoods.AuthService.Domain.Constants;
using FastTechFoods.AuthService.Domain.Entities;
using FastTechFoods.AuthService.Domain.Interfaces;
using FluentValidation;

namespace FastTechFoods.AuthService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly ITokenService _token;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<LoginRequest> _loginValidator;

        public UserService(
            IUserRepository repo,
            ITokenService token,
            IValidator<RegisterRequest> registerValidator,
            IValidator<LoginRequest> loginValidator)
        {
            _repo = repo;
            _token = token;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        public async Task<AuthResponse> AuthenticateAsync(LoginRequest request)
        {
            var validation = await _loginValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                var msg = string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage));
                return AuthResponse.Fail(msg);
            }

            var user = await _repo.GetByEmailOrCpfAsync(request.Login);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return AuthResponse.Fail("Credenciais inválidas");

            return AuthResponse.Success(user, _token.GenerateToken(user));
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var validation = await _registerValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                var msg = string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage));
                return AuthResponse.Fail(msg);
            }

            if (await _repo.ExistsAsync(request.Email, request.Cpf))
                return AuthResponse.Fail("Usuário já existe");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
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