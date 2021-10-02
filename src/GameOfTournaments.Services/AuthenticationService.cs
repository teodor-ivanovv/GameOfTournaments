namespace GameOfTournaments.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using GameOfTournaments.Services.Infrastructure;
    using GameOfTournaments.Shared;

    public class AuthenticationService : IAuthenticationService
    {
        public IAuthenticationContext Context { get; set; }

        public bool Authenticated => this.Context?.Authenticated ?? false;

        public void Set(IAuthenticationContext context) => this.Context = context;

        public bool HasPermissions(PermissionScope scope, Permissions permissions)
        {
            if (!this.Authenticated)
                return false;

            if (!this.Context.Permissions.Any(p => p.Scope == scope && p.Permissions.HasFlag(permissions)))
                return false;

            return true;
        }

        public bool HasPermissions(IEnumerable<PermissionModel> permissionModels)
        {
            if (!this.Authenticated)
                return false;

            foreach (var permission in permissionModels)
            {
                if (!this.HasPermissions(permission.Scope, permission.Permissions))
                    return false;
            }

            return true;
        }
    }
}
