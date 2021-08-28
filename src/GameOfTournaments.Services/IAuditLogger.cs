namespace GameOfTournaments.Services
{
    using GameOfTournaments.Data.Models;

    public interface IAuditLogger : IEfCoreService<AuditLog>
    {
    }
}