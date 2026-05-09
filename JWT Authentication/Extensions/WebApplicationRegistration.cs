using Microsoft.EntityFrameworkCore;
using Presistence.Data.DbContexts;

namespace JWT_Authentication.Extensions
{
    public static class WebApplicationRegistration
    {
        public static async Task<WebApplication> MigrateIdentityDatabase(this WebApplication application)
        {
            await using var scope = application.Services.CreateAsyncScope();
            var dbContextService = scope.ServiceProvider.GetRequiredService<StoreIdentityDbContext>();
            var penndingMigrations = await dbContextService.Database.GetPendingMigrationsAsync();
            if (penndingMigrations.Any())
                await dbContextService.Database.MigrateAsync();

            return application;
        }
    }
}
