using FastTechFoods.AuthService.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace FastTechFoods.AuthService.Infrastructure
{
    public static class DbInitializer
    {
        public static void Seed(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

            if (!context.Users.Any())
            {
                context.Users.Add(new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Administrador",
                    Email = "admin@fasttechfoods.com",
                    Cpf = "12345678910",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@123"),
                    Role = "Gerente"
                });
                context.SaveChanges();
            }
        }
    }
}
