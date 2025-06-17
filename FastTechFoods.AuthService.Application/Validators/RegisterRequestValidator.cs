using FastTechFoods.AuthService.Application.DTOs;
using FastTechFoods.AuthService.Domain.Constants;
using FluentValidation;

namespace FastTechFoods.AuthService.Application.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name é obrigatório.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório.")
                .EmailAddress().WithMessage("Email inválido.");

            RuleFor(x => x.Cpf)
                .NotEmpty().WithMessage("CPF é obrigatório.")
                .Length(11).WithMessage("CPF deve ter 11 dígitos.")
                .Matches("^[0-9]*$").WithMessage("CPF deve conter apenas números.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Senha é obrigatória.")
                .MinimumLength(6).WithMessage("Senha deve ter pelo menos 6 caracteres.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Perfil é obrigatório.")
                .Must(r => UserRoles.Todos.Contains(r)).WithMessage("Perfil inválido.");
        }
    }
}
