using FastTechFoods.AuthService.Application.Interfaces;
using FastTechFoods.AuthService.Application.Services;
using FastTechFoods.AuthService.Application.Validators;
using FastTechFoods.AuthService.Domain.Interfaces;
using FastTechFoods.AuthService.Infrastructure;
using FastTechFoods.AuthService.Infrastructure.Repositories;
using FastTechFoods.AuthService.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace FastTechFoods.AuthService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_DATABASE") ??
                builder.Configuration.GetConnectionString("DefaultConnection");

            // DbContext
            builder.Services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Reposit�rios e Servi�os
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITokenService, TokenService>();

            // FluentValidation
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

            // Autentica��o JWT
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                            builder.Configuration["Jwt:Key"]!))
                    };
                });

            //Swagger
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                options.IncludeXmlComments(xmlPath);

                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Insira 'Bearer {seu token}'"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            //Migrations
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
                dbContext.Database.Migrate();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            DbInitializer.Seed(app.Services);

            app.Run();
        }
    }
}