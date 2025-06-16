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
                    Email = "master@fasttech.com",
                    Cpf = "09109395946",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@master"),
                    Role = "Gerente"
                });
                context.SaveChanges();
            }
        }
    }
}
