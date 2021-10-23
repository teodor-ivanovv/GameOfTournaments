namespace GameOfTournaments.Services
{
    using System;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using GameOfTournaments.Data;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Shared;
    using Microsoft.EntityFrameworkCore;
    using static Data.Infrastructure.DatabaseIntegrity;

    public class AuditLogger : IAuditLogger
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IAuthenticationService _authenticationService;

        public AuditLogger(IDbContextFactory<ApplicationDbContext> contextFactory, IAuthenticationService authenticationService)
        {
            this._contextFactory = contextFactory;
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
        
        private async Task<IOperationResult<AuditLog>> CreateAsync(AuditLog log)
        {
            var operationResult = new OperationResult<AuditLog>();
            operationResult.ValidateNotNull(log, nameof(AuditLogger), nameof(this.CreateAsync), nameof(log));

            if (!operationResult.Success)
                return operationResult;
            
            await using var dbContext = this._contextFactory.CreateDbContext();
            await dbContext.AddAsync(log);
            var affectedRows = await dbContext.SaveChangesAsync();

            ValidateAffectedRows(
                affectedRows,
                expected: 1,
                nameof(AuditLogger), 
                nameof(this.CreateAsync), 
                typeof(AuditLog).FullName);

            operationResult.AffectedRows = affectedRows;
            operationResult.Object = log;
            return operationResult;
        }
    }
}