namespace GameOfTournaments.Services
{
    using System;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services.Infrastructure;
    using Microsoft.AspNetCore.Identity;

    // TODO: Cache user and roles
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> userManager;
        
        public AuthenticationService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public IAuthenticationContext Context { get; set; }

        public bool Authenticated => this.Context?.ApplicationUser?.Identity?.IsAuthenticated ?? false;

        public void Set(IAuthenticationContext context) => this.Context = context;

        public async Task<bool> IsInRoleAsync(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return false;

            var user = await this.userManager.GetUserAsync(this.Context.ApplicationUser);
            return await this.userManager.IsInRoleAsync(user, role);
        }
    }
}