namespace GameOfTournaments.Web.ServiceCollectionExtensions
{
    using GameOfTournaments.Services;
    using Microsoft.Extensions.DependencyInjection;

    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
            => services.AddScoped<IJwtService, JwtService>();
    }
}