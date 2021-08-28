namespace GameOfTournaments.Seeder
{
    using System;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using static Shared.SystemConstants;

    public class SystemUserSeeder : ISystemUserSeeder
    {
        private readonly UserManager<ApplicationUser> userManager;

        public SystemUserSeeder(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager)); 
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

            await this.userManager.CreateAsync(systemUser, Guid.NewGuid().ToString());
        }
    }
}