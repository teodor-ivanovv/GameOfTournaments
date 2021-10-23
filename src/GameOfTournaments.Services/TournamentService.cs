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

    public class TournamentService : EfCoreService<Tournament>, ITournamentService
    {
        private const PermissionScope Scope = PermissionScope.Tournament;

        private readonly IAuthenticationService _authenticationService;

        private readonly IApplicationUserAccountService _applicationUserAccountService;

        public TournamentService(
            IDbContextFactory<ApplicationDbContext> contextFactory, 
            IAuthenticationService authenticationService,
            IApplicationUserAccountService applicationUserAccountService,
            IAuditLogger auditLogger)
            : base(contextFactory, authenticationService, auditLogger)
        {
            this._authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            this._applicationUserAccountService = applicationUserAccountService ?? throw new ArgumentNullException(nameof(applicationUserAccountService));
        }
        
        // TODO: Validate tournaments per day rate limit

        public override async Task<IOperationResult<Tournament>> CreateAsync(Tournament entity, CancellationToken cancellationToken = default)
        {
            var operationResult = this.ValidatePermissions(entity, Scope, Permissions.Create);
            if (!operationResult.Success)
                return operationResult;

            var canCreateTournamentResult = await this.ValidateUserCanCreateTournamentAsync(cancellationToken);
            if (!canCreateTournamentResult.Success)
                return canCreateTournamentResult;

            return await base.CreateAsync(entity, cancellationToken);
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

        private async Task<OperationResult<Tournament>> ValidateUserCanCreateTournamentAsync(CancellationToken cancellationToken)
        {
            var createdTournaments = await this.CountAsync(
                t => t.CreatedBy == this._authenticationService.Context.Id && t.Created.Date == DateTime.UtcNow.Date);

            var tournamentsPerDay = await this._applicationUserAccountService.GetAsync(
                new GetOptions<ApplicationUserAccount, int, int>
                {
                    FilterExpression = userAccount => userAccount.ApplicationUserId == this._authenticationService.Context.Id,
                    Projection = new ProjectionOptions<ApplicationUserAccount, int>(userAccount => userAccount.CreateTournamentsPerDay),
                },
                cancellationToken);

            var operationResult = new OperationResult<Tournament>();
            var tournamentsPerDayCount = tournamentsPerDay.Count == 1 ? tournamentsPerDay.FirstOrDefault() : 10;
            if (createdTournaments > tournamentsPerDayCount)
                operationResult.AddErrorMessage($"You cannot create more tournaments for today. Currently you can create up to {tournamentsPerDayCount} tournaments. Upgrade your subscription in order to crete more tournaments.");

            return operationResult;
        }
    }
}