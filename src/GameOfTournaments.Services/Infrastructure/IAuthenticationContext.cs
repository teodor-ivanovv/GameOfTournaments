namespace GameOfTournaments.Services.Infrastructure
{
    using System.Collections.Generic;
    using System.Security.Claims;

    public interface IAuthenticationContext
    {
        int Id { get; }
        
        string IpAddress { get; }
        
        bool Authenticated { get; set; }
        
        ClaimsPrincipal ApplicationUser { get; set; }
        
        List<string> Roles { get; }

        // Logins, time
    }
}