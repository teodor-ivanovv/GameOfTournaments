namespace GameOfTournaments.Services.Infrastructure
{
    using System.Security.Claims;
    using GameOfTournaments.Data.Models;

    public interface IAuthenticationContext
    {
        int Id { get; }
        
        string IpAddress { get; }
        
        ClaimsPrincipal ApplicationUser { get; set; }

        // Logins, time
    }
}