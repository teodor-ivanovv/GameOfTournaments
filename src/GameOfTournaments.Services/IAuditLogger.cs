namespace GameOfTournaments.Services
{
    using System;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Shared;

    public interface IAuditLogger
    {
        Task LogAsync(AuditLog log);

        AuditLog Construct(PermissionScope scope, Permissions permissions, DateTimeOffset actionTime, string message, bool hasPermissions);
        
        AuditLog Construct<T>(PermissionScope scope, Permissions permissions, DateTimeOffset actionTime, string message, T entityId, bool hasPermissions);

        Action ConstructLogAction(bool hasPermissions, PermissionScope scope, Permissions permissions);

        Action ConstructLogAction<T>(bool hasPermissions, PermissionScope scope, Permissions permissions, T entityId);
    }
}