namespace GameOfTournaments.Services.Infrastructure
{
    using System.Security.Claims;

    public class AuthenticationContext : IAuthenticationContext
    {
        public int Id { get; set; }

        public string IpAddress { get; set; }

        public ClaimsPrincipal ApplicationUser { get; set; }
    }
}