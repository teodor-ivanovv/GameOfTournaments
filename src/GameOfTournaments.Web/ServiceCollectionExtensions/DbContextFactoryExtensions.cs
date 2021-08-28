namespace GameOfTournaments.Web.ServiceCollectionExtensions
{
    using GameOfTournaments.Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class DbContextFactoryExtensions
    {
        public static IServiceCollection RegisterDbContextFactory(this IServiceCollection services, ApplicationSettings applicationSettings)
        {
             services.AddDbContextFactory<ApplicationDbContext>(
                 options => options.UseNpgsql(applicationSettings.ConnectionString));

             return services;
        }
    }
}