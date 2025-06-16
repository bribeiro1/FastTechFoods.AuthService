using FastTechFoods.AuthService.Application.DTOs;
using FastTechFoods.AuthService.Application.Interfaces;
using FastTechFoods.AuthService.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace FastTechFoods.AuthService.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => _authService = authService;

        /// <summary>
        /// Realiza o login de um usuário com CPF ou Email e senha.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.AuthenticateAsync(request);
            return result.IsAuthenticated ? Ok(result) : Unauthorized(result);
        }

        /// <summary>
        /// Registra um novo usuário. Apenas clientes podem se registrar livremente.
        /// Para outros papéis (Atendente, Gerente), é necessário estar autenticado como Gerente.
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!UserRoles.Todos.Contains(request.Role))
                return BadRequest("Role inválida");

            if (request.Role != UserRoles.Cliente)
            {
                if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole(UserRoles.Gerente))
                    return Forbid("Apenas um gerente pode registrar esse tipo de usuário.");
            }

            var result = await _authService.RegisterAsync(request);
            return result.IsAuthenticated ? Ok(result) : BadRequest("Erro ao registrar usuário");
        }
    }
}
