namespace GameOfTournaments.Services
{
    using System;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using GameOfTournaments.Data;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Shared;
    using Microsoft.EntityFrameworkCore;

    public class AuditLogger : EfCoreService<AuditLog>, IAuditLogger
    {
        private readonly IAuthenticationService _authenticationService;

        public AuditLogger(IDbContextFactory<ApplicationDbContext> contextFactory, IAuthenticationService authenticationService)
            : base(contextFactory, authenticationService)
        {
            this._authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        /// <inheritdoc />
        public Task LogAsync(AuditLog log)
        {
            Guard.Against.Null(log, nameof(log));
            return this.CreateAsync(log);
        }

        public AuditLog Construct(string action, DateTimeOffset actionTime, string message, bool hasPermissions)
        {
            var auditLog = new AuditLog
            {
                Action = action,
                ActionTime = actionTime,
                Message = message,
                HasPermissions = hasPermissions,
                IpAddress = this._authenticationService.Context?.IpAddress,
                UserId = this._authenticationService.Context?.Id,
            };

            return auditLog;
        }

        public AuditLog Construct<T>(string action, DateTimeOffset actionTime, string message, T entityId, bool hasPermissions)
        {
            var auditLog = this.Construct(action, actionTime, message, hasPermissions);
            auditLog.EntityId = entityId.ToString();

            return auditLog;
        }

        public Action ConstructLogAction(bool inRole, string role)
            => this.LogAsync(
                this.Construct(
                    role, 
                    DateTimeOffset.UtcNow, 
                    string.Empty,
                    inRole))
                .ExecuteNonBlocking;
        
        public Action ConstructLogAction<T>(bool inRole, string role, T entityId)
            => this.LogAsync(
                    this.Construct(
                        role, 
                        DateTimeOffset.UtcNow, 
                        string.Empty,
                        entityId,
                        inRole))
                .ExecuteNonBlocking;
    }
}