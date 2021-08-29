namespace GameOfTournaments.Seeder
{
    using System;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services;
    using GameOfTournaments.Shared;
    using Microsoft.AspNetCore.Identity;
    using static Shared.Actions; 

    public class RoleSeeder : IRoleSeeder
    {
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger logger;

        public RoleSeeder(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, ILogger logger)
        {
            this.roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> AlreadySeededAsync()
        {
            var gameRole = await this.roleManager.FindByNameAsync(GameCreatorRoleName);
            return gameRole != null;
        }

        public async Task SeedAsync()
        {
            const string systemUsername = SystemConstants.SystemUsername;
            var systemUser = await this.userManager.FindByNameAsync(systemUsername);

            if (systemUser == null)
                throw new InvalidOperationException("A system user cannot be found and roles cannot be seeded.");
            
            // Game creator
            await this.SeedGameCreatorAsync(systemUser);
        }

        private async Task SeedGameCreatorAsync(ApplicationUser systemUser)
        {
            var gameCreator = new ApplicationRole
            {
                Action = GameCreatorRoleName, 
                Name = GameCreatorRoleName,
                Created = DateTimeOffset.UtcNow,
                CreatedBy = systemUser.Id, 
                LastModifiedBy = systemUser.Id,
            };

            var result = await this.roleManager.CreateAsync(gameCreator);
            if (!result.Succeeded)
                await this.logger.LogAsync($"Seeding game role did not succeed. {string.Join(',', result.Errors)}", LogSeverity.Error);
        }
    }
}