namespace GameOfTournaments.Services.Infrastructure
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Shared;

    public interface IAuthenticationContext
    {
        int Id { get; }
        
        string IpAddress { get; }
        
        bool Authenticated { get; set; }
        
        ClaimsPrincipal ApplicationUser { get; set; }
        
        List<PermissionModel> Permissions { get; }

        // Logins, time
    }
}