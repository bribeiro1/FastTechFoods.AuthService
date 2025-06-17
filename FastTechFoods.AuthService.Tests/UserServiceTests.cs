using Moq;
using FluentValidation;
using FluentValidation.Results;
using FastTechFoods.AuthService.Application.DTOs;
using FastTechFoods.AuthService.Application.Services;
using FastTechFoods.AuthService.Domain.Entities;
using FastTechFoods.AuthService.Domain.Interfaces;
using FastTechFoods.AuthService.Application.Interfaces;

namespace FastTechFoods.AuthService.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _repoMock;
        private readonly Mock<ITokenService> _tokenMock;
        private readonly Mock<IValidator<RegisterRequest>> _registerValidatorMock;
        private readonly Mock<IValidator<LoginRequest>> _loginValidatorMock;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _repoMock = new Mock<IUserRepository>();
            _tokenMock = new Mock<ITokenService>();
            _registerValidatorMock = new Mock<IValidator<RegisterRequest>>();
            _loginValidatorMock = new Mock<IValidator<LoginRequest>>();

            _service = new UserService(
                _repoMock.Object,
                _tokenMock.Object,
                _registerValidatorMock.Object,
                _loginValidatorMock.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnFail_WhenValidationFails()
        {
            var request = new RegisterRequest();
            var failures = new List<ValidationFailure> { new("Email", "Email inválido") };
            _registerValidatorMock.Setup(v => v.ValidateAsync(request, default))
                                  .ReturnsAsync(new ValidationResult(failures));

            var result = await _service.RegisterAsync(request);

            Assert.False(result.IsAuthenticated);
            Assert.Contains("Email inválido", result.Message);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnFail_WhenUserNotFound()
        {
            var request = new LoginRequest { Login = "user@test.com", Password = "123456" };
            _loginValidatorMock.Setup(v => v.ValidateAsync(request, default))
                               .ReturnsAsync(new ValidationResult());

            _repoMock.Setup(r => r.GetByEmailOrCpfAsync(request.Login))
                     .ReturnsAsync((User)null);

            var result = await _service.AuthenticateAsync(request);

            Assert.False(result.IsAuthenticated);
            Assert.Equal("Credenciais inválidas", result.Message);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
        {
            var request = new LoginRequest { Login = "user@test.com", Password = "123456" };
            var user = new User { Email = "user@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), Role = "Cliente" };

            _loginValidatorMock.Setup(v => v.ValidateAsync(request, default))
                               .ReturnsAsync(new ValidationResult());
            _repoMock.Setup(r => r.GetByEmailOrCpfAsync(request.Login))
                     .ReturnsAsync(user);
            _tokenMock.Setup(t => t.GenerateToken(user)).Returns("token123");

            var result = await _service.AuthenticateAsync(request);

            Assert.True(result.IsAuthenticated);
            Assert.Equal("token123", result.Token);
            Assert.Equal("Cliente", result.Role);
        }
    }
}
