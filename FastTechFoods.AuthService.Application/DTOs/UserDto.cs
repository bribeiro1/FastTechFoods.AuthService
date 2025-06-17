using FastTechFoods.AuthService.Domain.Entities;

namespace FastTechFoods.AuthService.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public UserDto(User user)
        {
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
            Cpf = user.Cpf;
            Role = user.Role;
        }
    }
}
