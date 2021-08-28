namespace GameOfTournaments.Web.ServiceCollectionExtensions
{
    using Ardalis.GuardClauses;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ApplicationSettingsExtensions
    {
        public static ApplicationSettings GetApplicationSettings(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            Guard.Against.Null(configuration, nameof(configuration));
            
            var applicationSettingsConfiguration = configuration.GetSection("ApplicationSettings");
            services.Configure<ApplicationSettings>(applicationSettingsConfiguration);
            
            return applicationSettingsConfiguration.Get<ApplicationSettings>();
        }
    }
}