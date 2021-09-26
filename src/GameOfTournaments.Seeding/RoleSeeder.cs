namespace GameOfTournaments.Seeder
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services;
    using GameOfTournaments.Shared;
    using Microsoft.AspNetCore.Identity;
    using static Shared.Permissions; 

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

        public Task<bool> AlreadySeededAsync() => Task.FromResult(false);

        public async Task SeedAsync()
        {
            const string systemUsername = SystemConstants.SystemUsername;
            var systemUser = await this.userManager.FindByNameAsync(systemUsername);

            if (systemUser == null)
                throw new InvalidOperationException("A system user cannot be found and roles cannot be seeded.");

            // Game creator
            await this.EnsureRoleAsync(systemUser, CanCreateGame);

            // Game editor
            await this.EnsureRoleAsync(systemUser, CanUpdateGame);
            
            // Game delete role
            await this.EnsureRoleAsync(systemUser, CanDeleteGame);
        }

        private async Task EnsureRoleAsync(ApplicationUser systemUser, string roleName)
        {
            var exists = await this.roleManager.RoleExistsAsync(roleName);
            if (exists)
                return;
            
            var role = new ApplicationRole
            {
                Action = roleName, 
                Name = roleName,
                Created = DateTimeOffset.UtcNow,
                CreatedBy = systemUser.Id, 
                LastModifiedBy = systemUser.Id,
            };

            var result = await this.roleManager.CreateAsync(role);
            if (!result.Succeeded)
                await this.logger.LogAsync($"Seeding {roleName} role did not succeed. {string.Join(',', result.Errors)}", LogSeverity.Error);
        }
    }
}