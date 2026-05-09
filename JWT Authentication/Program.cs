using Domain.Contracts;
using Domain.Entities.IdentitiyModule;
using JWT_Authentication.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Presistence.Data.DataSeed;
using Presistence.Data.DbContexts;
using Services;
using Services.Abstraction;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Text;
using Shared;

namespace JWT_Authentication
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            // Register StoreIdentityDbContext in the dependency injection container
            builder.Services.AddDbContext<StoreIdentityDbContext>(options =>
            {
                // Configure Entity Framework Core to use SQL Server as the database provider
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

            // Register IdentityDataInitializer as a keyed scoped service
            // using the key "Default" for dependency resolution
            builder.Services.AddKeyedScoped<IDataInitializer, IdentityDataInitializer>("Identity");

            /* Register ASP.NET Core Identity services using ApplicationUser as the user entity
             * Enable role management with IdentityRole
             * Configure Identity to use StoreIdentityDbContext
             * for storing users, roles, and authentication data */
            builder.Services.AddIdentityCore<ApplicationUser>().AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<StoreIdentityDbContext>();


            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWTOptions"));
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["JWTOptions:Issuer"],
                    ValidAudience = builder.Configuration["JWTOptions:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTOptions:SecretKey"]!)),
                };
            });





            var app = builder.Build();

            #region Pennding Migrations - Seeding

            await app.MigrateIdentityDatabase();
            await app.IdentitySeedDatabaseAsync();

            #endregion



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
