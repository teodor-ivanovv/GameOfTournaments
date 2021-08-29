namespace GameOfTournaments.Web.ServiceCollectionExtensions
{
    using GameOfTournaments.Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class MigrationsExtensions
    {
        public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder applicationBuilder)
        {
            using var scope = applicationBuilder.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
            dbContext?.Database.Migrate();

            return applicationBuilder;
        }
    }
}
