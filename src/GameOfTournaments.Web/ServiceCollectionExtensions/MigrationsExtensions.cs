namespace GameOfTournaments.Web.ServiceCollectionExtensions
{
    using GameOfTournaments.Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class MigrationsExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
            dbContext?.Database.Migrate();
        }
    }
}
