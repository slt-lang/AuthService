using AuthService.Adapters.Database;
using AuthService.Domain;
using AuthService.Domain.Logic;
using AuthService.Domain.Ports;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using sltlang.Common.AuthService.Enums;
using sltlang.Common.Common.Extensions;
using System.Text;

namespace AuthService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddNpgsql<AuthServiceContext>(builder.Configuration.GetConnectionString("AuthDb"));
            builder.Services.AddTransient<IAuthDb, AuthDb>();
            builder.Services.AddTransient<IDateTime, DateTimeProvider>();
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));

            var configuration = builder.Configuration.GetSection("Config").Get<Config>();
            builder.Services.AddSingleton(configuration ?? new());

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            EncodingProvider provider = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(provider);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration?.JwtSettings.Issuer,
                        ValidAudience = configuration?.JwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration?.JwtSettings.Secret))
                    };
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            //using (var scope = app.Services.CreateScope())
            //{
            //    var db = scope.ServiceProvider.GetRequiredService<AuthServiceContext>();
            //    await db.Database.MigrateAsync();
            //}

            app.Run();
        }
    }
}
