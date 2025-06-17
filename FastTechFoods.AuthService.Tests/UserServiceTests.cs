using FastTechFoods.AuthService.Application.DTOs;
using FastTechFoods.AuthService.Application.Interfaces;
using FastTechFoods.AuthService.Application.Services;
using FastTechFoods.AuthService.Domain.Entities;
using FastTechFoods.AuthService.Domain.Interfaces;
using Moq;

namespace FastTechFoods.AuthService.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly IAuthService _authService;

        public UserServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _authService = new UserService(
                _userRepoMock.Object,
                _tokenServiceMock.Object
            );
        }

        [Fact]
        public async Task AuthenticateAsync_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@test.com",
                Cpf = "12345678900",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
                Role = "Cliente"
            };

            _userRepoMock.Setup(r => r.GetByEmailOrCpfAsync("user@test.com"))
                         .ReturnsAsync(user);

            _tokenServiceMock.Setup(t => t.GenerateToken(user))
                             .Returns("fake-jwt-token");

            var request = new LoginRequest
            {
                Login = "user@test.com",
                Password = "1234"
            };

            // Act
            var result = await _authService.AuthenticateAsync(request);

            // Assert
            Assert.True(result.IsAuthenticated);
            Assert.Equal("Cliente", result.Role);
            Assert.Equal("fake-jwt-token", result.Token);
        }

        [Fact]
        public async Task AuthenticateAsync_WithInvalidPassword_ReturnsFail()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@test.com",
                Cpf = "12345678900",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct"),
                Role = "Cliente"
            };

            _userRepoMock.Setup(r => r.GetByEmailOrCpfAsync("user@test.com"))
                         .ReturnsAsync(user);

            var request = new LoginRequest
            {
                Login = "user@test.com",
                Password = "wrong"
            };

            var result = await _authService.AuthenticateAsync(request);

            Assert.False(result.IsAuthenticated);
            Assert.Null(result.Token);
        }

        [Fact]
        public async Task RegisterAsync_WithNewUser_ReturnsSuccess()
        {
            var request = new RegisterRequest
            {
                Email = "new@user.com",
                Cpf = "99999999999",
                Password = "abc123",
                Role = "Cliente"
            };

            _userRepoMock.Setup(r => r.ExistsAsync(request.Email, request.Cpf))
                         .ReturnsAsync(false);

            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<User>()))
                             .Returns("new-token");

            var result = await _authService.RegisterAsync(request);

            Assert.True(result.IsAuthenticated);
            Assert.Equal("Cliente", result.Role);
            Assert.Equal("new-token", result.Token);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingUser_ReturnsFail()
        {
            var request = new RegisterRequest
            {
                Email = "exist@user.com",
                Cpf = "88888888888",
                Password = "pass",
                Role = "Cliente"
            };

            _userRepoMock.Setup(r => r.ExistsAsync(request.Email, request.Cpf))
                         .ReturnsAsync(true);

            var result = await _authService.RegisterAsync(request);

            Assert.False(result.IsAuthenticated);
            Assert.Null(result.Token);
        }
    }
}
