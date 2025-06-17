using FastTechFoods.AuthService.Application.DTOs;
using FluentValidation;

namespace FastTechFoods.AuthService.Application.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Login).NotEmpty().WithMessage("Login é obrigatório.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Senha é obrigatória.");
        }
    }
}
