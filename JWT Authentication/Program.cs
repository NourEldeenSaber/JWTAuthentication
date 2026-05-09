using JWT_Authentication.Extensions;
using Microsoft.EntityFrameworkCore;
using Presistence.Data.DbContexts;
using System.Threading.Tasks;

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


            var app = builder.Build();

            #region Pennding Migrations

            await app.MigrateIdentityDatabase();

            #endregion



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
