namespace GameOfTournaments.Web.ServiceCollectionExtensions
{
    using System.Collections.Generic;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Seeder;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    public static class SeedingExtensions
    {
        public static IServiceCollection RegisterSeeders(this IServiceCollection services)
        {
            services
                .AddScoped<ISeedManager, SeedManager>()
                .AddScoped<ISystemUserSeeder, SystemUserSeeder>()
                .AddScoped<IRoleSeeder, RoleSeeder>();
            
            return services;
        }
        
        public static void SeedDefaultData(
            this IApplicationBuilder app,
            ISeedManager seedManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            var seeders = new List<ISeeder>
            {
                new SystemUserSeeder(userManager),
                new RoleSeeder(roleManager, userManager),
            };

            seedManager.SeedAsync(seeders);
        }
    }
}