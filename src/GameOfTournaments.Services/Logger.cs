namespace GameOfTournaments.Services
{
    using System;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using GameOfTournaments.Data;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class Logger : EfCoreService<Log>, ILogger
    {
        public Logger(IDbContextFactory<ApplicationDbContext> contextFactory, IAuthenticationService authenticationService)
            : base(contextFactory, authenticationService)
        {
        }
        
        // Introduce component to track ip

        public Task LogAsync(string message, LogSeverity severity)
        {
            Guard.Against.NullOrWhiteSpace(message, nameof(message));

            var log = new Log(message, severity);
            log.Time = DateTimeOffset.UtcNow;

            return this.CreateAsync(log);
        }

        public Task LogAsync(Log log)
        {
            Guard.Against.Null(log, nameof(log));
            
            if (log.Time == DateTimeOffset.MinValue)
                log.Time = DateTimeOffset.UtcNow;
            
            return this.CreateAsync(log);
        }
    }
}