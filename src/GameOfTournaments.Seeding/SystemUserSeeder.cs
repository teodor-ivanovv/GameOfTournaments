namespace GameOfTournaments.Seeder
{
    using System;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services;
    using Microsoft.AspNetCore.Identity;
    using static Shared.SystemConstants;

    public class SystemUserSeeder : ISystemUserSeeder
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger logger;

        public SystemUserSeeder(UserManager<ApplicationUser> userManager, ILogger logger)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> AlreadySeededAsync()
        {
            var systemUser = await this.userManager.FindByNameAsync(SystemUsername);
            return systemUser != null;
        }

        public async Task SeedAsync()
        {
            var systemUser = new ApplicationUser
            {
                Age = 20,
                Email = "support@gameoftournaments.net",
                Male = true,
                FirstName = "System",
                MiddleName = "System",
                LastName= "System",
                PhoneNumber = "+359 569 445 1111",
                UserName = SystemUsername,
            };

            var result = await this.userManager.CreateAsync(systemUser, Guid.NewGuid().ToString());
            if (!result.Succeeded)
                await this.logger.LogAsync($"Seeding system user did not succeeded. {string.Join(',', result.Errors)}", LogSeverity.Error);
        }
    }
}