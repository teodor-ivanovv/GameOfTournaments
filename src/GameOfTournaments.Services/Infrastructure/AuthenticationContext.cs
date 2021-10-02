namespace GameOfTournaments.Services.Infrastructure
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Shared;

    public class AuthenticationContext : IAuthenticationContext
    {
        private readonly List<PermissionModel> _permissions;

        public AuthenticationContext()
        {
            this._permissions = new List<PermissionModel>();
        }

        public AuthenticationContext(bool authenticated)
        : this()
        {
            this.Authenticated = authenticated;
        }

        public AuthenticationContext(bool authenticated, int id)
            : this(authenticated)
        {
            this.Id = id;
        }
        
        public int Id { get; set; }

        public string IpAddress { get; set; }

        public bool Authenticated { get; set; }

        public ClaimsPrincipal ApplicationUser { get; set; }

        public List<PermissionModel> Permissions
        {
            get => this._permissions;
            init
            {
                if (value is not null)
                    this._permissions = value;
            }
        }
    }
}