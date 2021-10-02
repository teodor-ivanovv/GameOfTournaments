namespace GameOfTournaments.Services
{
    using System.Collections.Generic;
    using GameOfTournaments.Services.Infrastructure;
    using GameOfTournaments.Shared;

    public interface IAuthenticationService
    {
        IAuthenticationContext Context { get; set; }
        
        bool Authenticated { get; }

        void Set(IAuthenticationContext context);

        bool HasPermissions(PermissionScope scope, Permissions permissions);
        
        bool HasPermissions(IEnumerable<PermissionModel> permissionModels);
    }
}