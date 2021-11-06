namespace GameOfTournaments.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
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

        public override Task<IOperationResult<List<TProjection>>> GetAsync<TSortKey, TProjection>(IGetOptions<Tournament, TSortKey, TProjection> getOptions, CancellationToken cancellationToken = default)
        {
            var operationResult = this.ValidateUserIsAuthenticated<List<TProjection>>();
            if (!operationResult.Success)
                return Task.FromResult(operationResult);

            // Apply public tournaments filter
            this.ApplyPublicFilter(getOptions);
            return base.GetAsync(getOptions, cancellationToken);
        }

        public override Task<IOperationResult<List<Tournament>>> GetAsync<TSortKey>(IGetOptions<Tournament, TSortKey> getOptions, CancellationToken cancellationToken = default)
        {
            var operationResult = this.ValidateUserIsAuthenticated<List<Tournament>>();
            if (!operationResult.Success)
                return Task.FromResult(operationResult);
            
            // Apply public tournaments filter
            this.ApplyPublicFilter(getOptions);
            return base.GetAsync(getOptions, cancellationToken);
        }

        public override Task<IOperationResult<List<Tournament>>> GetAsync(IGetOptions<Tournament> getOptions, CancellationToken cancellationToken = default)
        {
            var operationResult = this.ValidateUserIsAuthenticated<List<Tournament>>();
            if (!operationResult.Success)
                return Task.FromResult(operationResult);
            
            // Apply public tournaments filter
            this.ApplyPublicFilter(getOptions);
            return base.GetAsync(getOptions, cancellationToken);
        }

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
        
        public override async Task<IOperationResult<IEnumerable<Tournament>>> CreateManyAsync(IEnumerable<Tournament> entities, CancellationToken cancellationToken = default)
        {
            var enumerated = entities.EnsureCollectionToList();
            var operationResult = this.ValidatePermissions(enumerated, Scope, Permissions.Create);
            if (!operationResult.Success)
                return operationResult;
            
            var canCreateTournamentsResult = await this.GetUserRemainingTournamentsCreationAsync(enumerated.Count, cancellationToken);

            if (!canCreateTournamentsResult.Success)
            {
                operationResult.AddOperationResult(canCreateTournamentsResult);
                return operationResult;
            }
            
            return await base.CreateManyAsync(enumerated, cancellationToken);
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
            var now = DateTime.UtcNow;
            var operationResult = new OperationResult<Tournament>();
            var createdTournaments = await this.CountAsync(
                t => t.CreatedBy == this._authenticationService.Context.Id 
                    && t.Created.Year == now.Year
                    && t.Created.Month == now.Month
                    && t.Created.Day == now.Day);

            var tournamentsPerDayOperationResult = await this._applicationUserAccountService.GetAsync(
                new GetOptions<ApplicationUserAccount, int, int>
                {
                    FilterExpression = userAccount => userAccount.ApplicationUserId == this._authenticationService.Context.Id,
                    Projection = new ProjectionOptions<ApplicationUserAccount, int>(userAccount => userAccount.CreateTournamentsPerDay),
                },
                cancellationToken);

            if (!tournamentsPerDayOperationResult.Success)
            {
                operationResult.AddOperationResult(tournamentsPerDayOperationResult);
                return operationResult;
            }

            if (tournamentsPerDayOperationResult.Object.Count != 1)
            {
                operationResult.AddErrorMessage("Current user doesn't have enabled tournament creation permissions or creation configuration is not appropriately set at this stage.");
                return operationResult;
            }

            var tournamentsPerDayCount = tournamentsPerDayOperationResult.Object.FirstOrDefault();
            if (createdTournaments >= tournamentsPerDayCount)
                operationResult.AddErrorMessage($"You cannot create more tournaments for today. Currently you can create up to {tournamentsPerDayCount} tournaments. Today you have created {createdTournaments} tournaments. Upgrade your subscription in order to crete more tournaments.");

            return operationResult;
        }
        
        private async Task<OperationResult<int>> GetUserRemainingTournamentsCreationAsync(int requestedTournamentsCount, CancellationToken cancellationToken)
        {
            var operationResult = new OperationResult<int>();
            var createdTournaments = await this.CountAsync(
                t => t.CreatedBy == this._authenticationService.Context.Id && t.Created.Equals(DateTime.UtcNow.Date));

            var tournamentsPerDayOperationResult = await this._applicationUserAccountService.GetAsync(
                new GetOptions<ApplicationUserAccount, int, int>
                {
                    FilterExpression = userAccount => userAccount.ApplicationUserId == this._authenticationService.Context.Id,
                    Projection = new ProjectionOptions<ApplicationUserAccount, int>(userAccount => userAccount.CreateTournamentsPerDay),
                },
                cancellationToken);

            if (!tournamentsPerDayOperationResult.Success)
            {
                operationResult.AddOperationResult(tournamentsPerDayOperationResult);
                return operationResult;
            }
            
            if (tournamentsPerDayOperationResult.Object.Count != 1)
            {
                operationResult.AddErrorMessage("Current user doesn't have enabled tournament creation permissions or creation configuration is not appropriately set at this stage.");
                return operationResult;
            }

            var tournamentsPerDayCount = tournamentsPerDayOperationResult.Object.FirstOrDefault();
            if (requestedTournamentsCount > tournamentsPerDayCount - createdTournaments)
                operationResult.AddErrorMessage($"You cannot create more tournaments for today. Currently you can create up to {tournamentsPerDayCount} tournaments. Today you have created {createdTournaments} tournaments. Upgrade your subscription in order to crete more tournaments.");

            return operationResult;
        }
        
        private void ApplyPublicFilter(IGetOptions<Tournament> getOptions)
        {
            var userId = this._authenticationService.Context?.Id ?? 0;
            
            Expression<Func<Tournament, bool>> publicFilter = t => t.Public;
            Expression<Func<Tournament, bool>> userFilter = t => !t.Public && t.CreatedBy == userId;
            
            var andFilter = getOptions.FilterExpression.AndAlso(publicFilter);
            var finalFilter = andFilter.OrElse(userFilter);
            
            getOptions.FilterExpression = finalFilter;
        }
    }
}