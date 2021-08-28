namespace GameOfTournaments.Services
{
    using GameOfTournaments.Data;
    using GameOfTournaments.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class AuditLogger : EfCoreService<AuditLog>, IAuditLogger
    {
        public AuditLogger(IDbContextFactory<ApplicationDbContext> contextFactory)
            : base(contextFactory)
        {
        }
    }
}