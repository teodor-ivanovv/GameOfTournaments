namespace GameOfTournaments.Web.ServiceCollectionExtensions
{
    using System.Collections.Generic;
    using Ardalis.GuardClauses;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Seeder;
    using GameOfTournaments.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    public static class SeedingExtensions
    {
        public static IServiceCollection RegisterSeeders(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddScoped<ILogger, Logger>()
                .AddScoped<ISeedManager, SeedManager>()
                .AddScoped<ISystemUserSeeder, SystemUserSeeder>()
                .AddScoped<IRoleSeeder, RoleSeeder>();
            
            return serviceCollection;
        }
        
        public static void SeedDefaultData(
            this IApplicationBuilder applicationBuilder,
            ILogger logger,
            ISeedManager seedManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            Guard.Against.Null(logger, nameof(logger));
            Guard.Against.Null(seedManager, nameof(seedManager));
            Guard.Against.Null(userManager, nameof(userManager));
            Guard.Against.Null(roleManager, nameof(roleManager));

            var seeders = new List<ISeeder>
            {
                new SystemUserSeeder(userManager, logger),
                new RoleSeeder(roleManager, userManager, logger),
            };

            seedManager.SeedAsync(seeders).GetAwaiter().GetResult();
        }
    }
}