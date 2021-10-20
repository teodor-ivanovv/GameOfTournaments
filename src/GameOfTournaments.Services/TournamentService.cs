namespace GameOfTournaments.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GameOfTournaments.Data;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Shared;
    using Microsoft.EntityFrameworkCore;

    public class TournamentService : EfCoreService<Tournament>, ITournamentService
    {
        private const PermissionScope Scope = PermissionScope.Tournament;

        public TournamentService(IDbContextFactory<ApplicationDbContext> contextFactory, IAuthenticationService authenticationService, IAuditLogger auditLogger)
            : base(contextFactory, authenticationService, auditLogger)
        {
        }
        
        // TODO: Validate tournaments per day rate limit

        public override Task<IOperationResult<Tournament>> CreateAsync(Tournament entity, CancellationToken cancellationToken = default)
        {
            var operationResult = this.ValidatePermissions(entity, Scope, Permissions.Create);
            if (!operationResult.Success)
                return Task.FromResult(operationResult);
            
            return base.CreateAsync(entity, cancellationToken);
        }
        
        public override Task<IOperationResult<IEnumerable<Tournament>>> CreateManyAsync(IEnumerable<Tournament> entities, CancellationToken cancellationToken = default)
        {
            // TODO: Ensure collection ToList
            var operationResult = this.ValidatePermissions(entities, Scope, Permissions.Create);
            if (!operationResult.Success)
                return Task.FromResult(operationResult);
            
            return base.CreateManyAsync(entities, cancellationToken);
        }

        public override Task<IOperationResult<Tournament>> UpdateAsync(IEnumerable<object> identifiers, Tournament entity, CancellationToken cancellationToken = default)
        {
            var operationResult = this.ValidatePermissions(entity, Scope, Permissions.Update, entity.Id);
            if (!operationResult.Success)
                return Task.FromResult(operationResult);
            
            return base.UpdateAsync(identifiers, entity, cancellationToken);
        }
    }
}