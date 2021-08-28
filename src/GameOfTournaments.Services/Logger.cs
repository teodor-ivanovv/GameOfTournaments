namespace GameOfTournaments.Services
{
    using GameOfTournaments.Data;
    using GameOfTournaments.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class Logger : EfCoreService<Log>, ILogger
    {
        public Logger(IDbContextFactory<ApplicationDbContext> contextFactory)
            : base(contextFactory)
        {
        }
    }
}