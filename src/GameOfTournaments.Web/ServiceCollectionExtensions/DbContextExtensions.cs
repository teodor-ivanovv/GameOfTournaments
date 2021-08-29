namespace GameOfTournaments.Web.ServiceCollectionExtensions
{
    using Ardalis.GuardClauses;
    using GameOfTournaments.Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class DbContextExtensions
    {
        public static IServiceCollection RegisterDbContext(this IServiceCollection serviceCollection, ApplicationSettings applicationSettings)
        {
            Guard.Against.Null(applicationSettings, nameof(applicationSettings));
            
            serviceCollection
                .AddDbContext<ApplicationDbContext>(
                    options => options.UseNpgsql(applicationSettings.ConnectionString));
            
            return serviceCollection;
        }
    }
}