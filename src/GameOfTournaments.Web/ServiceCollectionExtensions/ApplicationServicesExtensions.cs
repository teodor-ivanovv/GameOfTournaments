namespace GameOfTournaments.Web.ServiceCollectionExtensions
{
    using GameOfTournaments.Services;
    using GameOfTournaments.Web.Cache.ApplicationUsers;
    using Microsoft.Extensions.DependencyInjection;

    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection serviceCollection)
            => serviceCollection
                .AddSingleton<IApplicationUserCache, ApplicationUserCache>()
                .AddScoped<IJwtService, JwtService>()
                .AddScoped<ILogger, Logger>()
                .AddScoped<IAuditLogger, AuditLogger>()
                .AddScoped<IGameService, GameService>()
                .AddScoped<ITournamentService, TournamentService>()
                .AddScoped<ITagService, TagService>()
                .AddScoped<IBetService, BetService>()
                .AddScoped<INewsService, NewsService>();
    }
}