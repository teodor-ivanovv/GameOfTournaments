namespace GameOfTournaments.Web.ServiceCollectionExtensions
{
    using GameOfTournaments.Data;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services;
    using Microsoft.Extensions.DependencyInjection;

    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentity(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IAuthenticationService, AuthenticationService>();
            serviceCollection.AddIdentity<ApplicationUser, ApplicationRole>(
                    options =>
                    {
                        options.Password.RequiredLength = 6;
                        options.Password.RequireDigit = false;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                    })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            return serviceCollection;
        }
    }
}