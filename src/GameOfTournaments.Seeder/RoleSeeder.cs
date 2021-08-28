namespace GameOfTournaments.Seeder
{
    using System;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Shared;
    using Microsoft.AspNetCore.Identity;
    using static Shared.Actions; 

    public class RoleSeeder : IRoleSeeder
    {
        private readonly RoleManager<ApplicationRole> roleManager;

        private readonly UserManager<ApplicationUser> userManager;

        public RoleSeeder(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
        
        public async Task SeedAsync()
        {
            const string systemUsername = SystemConstants.SystemUsername;
            var systemUser = await this.userManager.FindByNameAsync(systemUsername);

            if (systemUser == null)
                throw new InvalidOperationException("A system user cannot be found and roles cannot be seeded.");
            
            // Game creator
            var gameCreator = new ApplicationRole
            {
                Action = CreateGame,
                Name = "Game creator",
                Created = DateTimeOffset.UtcNow,
                CreatedBy = systemUser.Id,
                LastModifiedBy = systemUser.Id,
            };

            await this.roleManager.CreateAsync(gameCreator);
        }
    }
}