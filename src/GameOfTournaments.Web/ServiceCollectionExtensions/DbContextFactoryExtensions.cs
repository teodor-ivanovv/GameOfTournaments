namespace GameOfTournaments.Web.ServiceCollectionExtensions
{
    using Ardalis.GuardClauses;
    using GameOfTournaments.Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class DbContextFactoryExtensions
    {
        public static IServiceCollection RegisterDbContextFactory(this IServiceCollection serviceCollection, ApplicationSettings applicationSettings)
        {
            Guard.Against.Null(applicationSettings, nameof(applicationSettings));

             serviceCollection.AddDbContextFactory<ApplicationDbContext>(
                 options => options.UseNpgsql(applicationSettings.ConnectionString));

             return serviceCollection;
        }
    }
}