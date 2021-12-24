namespace GameOfTournaments.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GameOfTournaments.Data;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services.Infrastructure;
    using GameOfTournaments.Shared;
    using Microsoft.EntityFrameworkCore;

    public class GameService : EfCoreDeletableService<Game>, IGameService
    {
        private const PermissionScope Scope = PermissionScope.Game;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameService"/> class.
        /// </summary>
        /// <param name="contextFactory"></param>
        /// <param name="authenticationService"></param>
        /// <param name="auditLogger"></param>
        public GameService(
            IDbContextFactory<ApplicationDbContext> contextFactory,
            IAuthenticationService authenticationService,
            IAuditLogger auditLogger)
            : base(contextFactory, authenticationService, auditLogger)
        {
        }

        /// <inheritdoc/>
        public override Task<IOperationResult<List<Game>>> GetAsync<TSortKey>(IGetOptions<Game, TSortKey> getOptions, CancellationToken cancellationToken = default)
        {
            var operationResult = this.ValidateUserIsAuthenticated<List<Game>>();
            if (!operationResult.Success)
                return Task.FromResult(operationResult);

            return base.GetAsync(getOptions, cancellationToken);
        }

        /// <inheritdoc/>
        public override Task<IOperationResult<List<TProjection>>> GetAsync<TSortKey, TProjection>(IGetOptions<Game, TSortKey, TProjection> getOptions, CancellationToken cancellationToken = default)
        {
            var operationResult = this.ValidateUserIsAuthenticated<List<TProjection>>();
            if (!operationResult.Success)
                return Task.FromResult(operationResult);

            return base.GetAsync(getOptions, cancellationToken);
        }

        /// <inheritdoc/>
        public override Task<IOperationResult<List<Game>>> GetAsync(IGetOptions<Game> getOptions, CancellationToken cancellationToken = default)
        {
            var operationResult = this.ValidateUserIsAuthenticated<List<Game>>();
            if (!operationResult.Success)
                return Task.FromResult(operationResult);

            return base.GetAsync(getOptions, cancellationToken);
        }

        /// <inheritdoc/>
        public override Task<IOperationResult<Game>> CreateAsync(Game entity, CancellationToken cancellationToken = default)
        {
            var operationResult = this.ValidatePermissions(entity, Scope, Permissions.Create);
            if (!operationResult.Success)
                return Task.FromResult(operationResult);

            return base.CreateAsync(entity, cancellationToken);
        }

        /// <inheritdoc/>
        public override Task<IOperationResult<IEnumerable<Game>>> CreateManyAsync(IEnumerable<Game> entities, CancellationToken cancellationToken = default)
        {
            var operationResult = this.ValidatePermissions(entities, Scope, Permissions.Create);
            if (!operationResult.Success)
                return Task.FromResult(operationResult);

            return base.CreateManyAsync(entities, cancellationToken);
        }

        /// <inheritdoc/>
        public override Task<IOperationResult<Game>> UpdateAsync(IEnumerable<object> identifiers, Game entity, CancellationToken cancellationToken = default)
        {
            var operationResult = this.ValidatePermissions(entity, Scope, Permissions.Update, entity.Id);
            if (!operationResult.Success)
                return Task.FromResult(operationResult);

            return base.UpdateAsync(identifiers, entity, cancellationToken);
        }

        /// <inheritdoc/>
        public override Task<IOperationResult<Game>> SoftDeleteAsync(IEnumerable<object> identifiers, CancellationToken cancellationToken = default)
        {
            var enumerated = identifiers?.ToList();
            var operationResult = this.ValidatePermissions(enumerated, Scope, Permissions.SoftDelete, enumerated);
            if (!operationResult.Success)
                return Task.FromResult(operationResult.ChangeObjectType(new Game()));

            return base.SoftDeleteAsync(enumerated, cancellationToken);
        }
    }
}