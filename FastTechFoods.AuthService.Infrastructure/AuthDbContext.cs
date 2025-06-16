using FastTechFoods.AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastTechFoods.AuthService.Infrastructure
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}