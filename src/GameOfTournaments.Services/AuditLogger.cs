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

        public AuditLogger(IDbContextFactory<ApplicationDbContext> contextFactory, IAuthenticationService authenticationService, IAuditLogger auditLogger)
            : base(contextFactory, authenticationService, auditLogger)
        {
            this._authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        /// <inheritdoc />
        public Task LogAsync(AuditLog log)
        {
            Guard.Against.Null(log, nameof(log));
            return this.CreateAsync(log);
        }

        public AuditLog Construct(PermissionScope scope, Permissions permissions, DateTimeOffset actionTime, string message, bool hasPermissions)
        {
            var auditLog = new AuditLog
            {
                Scope = scope,
                Permissions = permissions,
                ActionTime = actionTime,
                Message = message,
                HasPermissions = hasPermissions,
                IpAddress = this._authenticationService.Context?.IpAddress,
                UserId = this._authenticationService.Context?.Id,
            };

            return auditLog;
        }

        public AuditLog Construct<T>(PermissionScope scope, Permissions permissions, DateTimeOffset actionTime, string message, T entityId, bool hasPermissions)
        {
            var auditLog = this.Construct(scope, permissions, actionTime, message, hasPermissions);
            auditLog.EntityId = entityId.ToString();

            return auditLog;
        }

        public Action ConstructLogAction(bool hasPermissions, PermissionScope scope, Permissions permissions)
            => this.LogAsync(
                this.Construct(
                    scope,
                    permissions,
                    DateTimeOffset.UtcNow, 
                    string.Empty,
                    hasPermissions))
                .ExecuteNonBlocking;
        
        public Action ConstructLogAction<T>(bool hasPermissions, PermissionScope scope, Permissions permissions, T entityId)
            => this.LogAsync(
                    this.Construct(
                        scope,
                        permissions,
                        DateTimeOffset.UtcNow, 
                        string.Empty,
                        entityId,
                        hasPermissions))
                .ExecuteNonBlocking;
    }
}