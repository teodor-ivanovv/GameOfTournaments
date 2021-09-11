namespace GameOfTournaments.Services.Infrastructure
{
    using System.Collections.Generic;
    using System.Security.Claims;

    public class AuthenticationContext : IAuthenticationContext
    {
        private readonly List<string> _roles;

        public AuthenticationContext()
        {
            this._roles = new List<string>();
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

        public List<string> Roles
        {
            get => this._roles;
            init
            {
                if (value is not null)
                    this._roles = value;
            }
        }
    }
}