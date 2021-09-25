namespace GameOfTournaments.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GameOfTournaments.Data;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Shared;
    using Microsoft.EntityFrameworkCore;

    public class GameService : EfCoreService<Game>, IGameService
    {
        private readonly IAuthenticationService _authenticationService;

        private readonly IAuditLogger _auditLogger;

        public GameService(
            IDbContextFactory<ApplicationDbContext> contextFactory,
            IAuthenticationService authenticationService,
            IAuditLogger auditLogger)
            : base(contextFactory, authenticationService)
        {
            this._authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            this._auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger));
        }

        public override Task<IOperationResult<Game>> CreateAsync(Game entity, CancellationToken cancellationToken = default)
        {
            var operationResult = new OperationResult<Game>();
            operationResult.ValidateNotNull(entity, nameof(GameService), nameof(this.CreateAsync), nameof(entity));

            var inRole = this._authenticationService.IsInRole(Permissions.CanCreateGame);
            operationResult.ValidateInRole(inRole, Permissions.CanCreateGame, this._auditLogger.ConstructLogAction(inRole, Permissions.CanCreateGame));
            
            if (!operationResult.Success)
                return Task.FromResult<IOperationResult<Game>>(operationResult);
            
            return base.CreateAsync(entity, cancellationToken);
        }

        public override Task<IOperationResult<Game>> UpdateAsync(IEnumerable<object> identifiers, Game entity, CancellationToken cancellationToken = default)
        {
            var operationResult = new OperationResult<Game>();
            operationResult.ValidateNotNull(entity, nameof(GameService), nameof(this.UpdateAsync), nameof(entity));

            var inRole = this._authenticationService.IsInRole(Permissions.CanUpdateGame);
            operationResult.ValidateInRole(inRole, Permissions.CanUpdateGame, this._auditLogger.ConstructLogAction(inRole, Permissions.CanUpdateGame, entity.Id));
            
            if (!operationResult.Success)
                return Task.FromResult<IOperationResult<Game>>(operationResult);
            
            return base.UpdateAsync(identifiers, entity, cancellationToken);
        }
    }
}