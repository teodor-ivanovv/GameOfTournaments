namespace GameOfTournaments.Services
{
    using System;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Models;

    public interface IAuditLogger : IEfCoreService<AuditLog>
    {
        Task LogAsync(AuditLog log);

        AuditLog Construct(string action, DateTimeOffset actionTime, string message, bool hasPermissions);
        
        AuditLog Construct<T>(string action, DateTimeOffset actionTime, string message, T entityId ,bool hasPermissions);

        Action ConstructLogAction(bool inRole, string role);

        Action ConstructLogAction<T>(bool inRole, string role, T entityId);
    }
}