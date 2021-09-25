namespace GameOfTournaments.Services
{
    using System;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Models;

    public interface IAuditLogger : IEfCoreService<AuditLog>
    {
        Task LogAsync(AuditLog log);

        AuditLog Construct(string action, DateTimeOffset actionTime, string message, bool hasPermissions);

        Action ConstructLogAction(bool inRole, string role);
    }
}