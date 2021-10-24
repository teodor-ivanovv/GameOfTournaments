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
    using GameOfTournaments.Services.Infrastructure;
    using GameOfTournaments.Shared;
    using Microsoft.EntityFrameworkCore;
    using static Data.Infrastructure.DatabaseIntegrity;
    
    public abstract class EfCoreService<TEntity> : IEfCoreService<TEntity> where TEntity : class
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuditLogger _auditLogger;

        protected IDbContextFactory<ApplicationDbContext> ContextFactory { get; private set; }

        protected EfCoreService(
            IDbContextFactory<ApplicationDbContext> contextFactory,
            IAuthenticationService authenticationService,
            IAuditLogger auditLogger)
        {
            this.ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            this._authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            this._auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger));
        }
        
        /// <inheritdoc />
        public virtual async Task<IOperationResult<TEntity>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var operationResult = new OperationResult<TEntity>();
            operationResult.ValidateNotNull(entity, nameof(EfCoreService<TEntity>), nameof(this.CreateAsync), nameof(entity));
            if (!operationResult.Success)
                return operationResult;

            this.ApplyAuditInformation(entity);
            
            await using var dbContext = this.ContextFactory.CreateDbContext();
            await dbContext.AddAsync(entity, cancellationToken);
            var affectedRows = await dbContext.SaveChangesAsync(cancellationToken);

            ValidateAffectedRows(
                affectedRows,
                expected: 1,
                nameof(EfCoreService<TEntity>), 
                nameof(this.CreateAsync), 
                typeof(TEntity).FullName);

            operationResult.AffectedRows = affectedRows;
            operationResult.Object = entity;
            return operationResult;
        }

        /// <inheritdoc />
        public virtual async Task<IOperationResult<IEnumerable<TEntity>>> CreateManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            var operationResult = new OperationResult<IEnumerable<TEntity>>();
            var enumerated = entities?.ToList();

            operationResult.ValidateNotNull(enumerated, nameof(EfCoreService<TEntity>), nameof(this.CreateManyAsync), nameof(entities));
            
            if (!operationResult.Success)
                return operationResult;
            
            foreach (var entity in enumerated)
                this.ApplyAuditInformation(entity);
            
            await using var dbContext = this.ContextFactory.CreateDbContext();
            
            await dbContext.AddRangeAsync(enumerated, cancellationToken);
            var affectedRows = await dbContext.SaveChangesAsync(cancellationToken);

            ValidateAffectedRows(
                affectedRows,
                expected: enumerated.Count, 
                nameof(EfCoreService<TEntity>), 
                nameof(this.CreateAsync), 
                typeof(TEntity).FullName);

            operationResult.AffectedRows = affectedRows;
            operationResult.Object = enumerated;

            return operationResult;
        }

        /// <inheritdoc />
        public async Task<IOperationResult<TEntity>> GetAsync(IEnumerable<object> identifiers, CancellationToken cancellationToken = default)
        {
            var operationResult = new OperationResult<TEntity>();
            var enumerated = identifiers as object[] ?? identifiers.ToArray();
            operationResult.ValidateNotNull(enumerated, nameof(EfCoreService<TEntity>), nameof(this.GetAsync), nameof(enumerated));

            if (!operationResult.Success)
                return operationResult;
            
            await using var dbContext = this.ContextFactory.CreateDbContext();
            operationResult.Object = await dbContext.Set<TEntity>()
                .FindAsync(enumerated.ToArray(), cancellationToken: cancellationToken)
                .AsTask();

            return operationResult;
        }

        /// <inheritdoc />
        public virtual async Task<IOperationResult<List<TEntity>>> GetAsync(IGetOptions<TEntity> getOptions, CancellationToken cancellationToken = default)
        {
            var operationResult = new OperationResult<List<TEntity>>();
            operationResult.ValidateNotNull(getOptions, nameof(EfCoreService<TEntity>), nameof(this.GetAsync), nameof(getOptions));

            if (!operationResult.Success)
                return operationResult;

            await using var dbContext = this.ContextFactory.CreateDbContext();
            var queryable = dbContext.Set<TEntity>().AsQueryable();

            if (getOptions.FilterExpression != null)
                queryable = queryable.Where(getOptions.FilterExpression);

            operationResult.Object = await queryable.ToListAsync(cancellationToken: cancellationToken);
            return operationResult;
        }

        public virtual async Task<IOperationResult<List<TEntity>>> GetAsync<TSortKey>(IGetOptions<TEntity, TSortKey> getOptions, CancellationToken cancellationToken = default)
        {
            var operationResult = new OperationResult<List<TEntity>>();
            operationResult.ValidateNotNull(getOptions, nameof(EfCoreService<TEntity>), nameof(this.GetAsync), nameof(getOptions));
            
            if (!operationResult.Success)
                return operationResult;

            if (getOptions.Pagination != null && getOptions.Sort == null)
            {
                throw new ArgumentNullException(
                    nameof(getOptions),
                    "GetAsync<TSortKey>(IGetOptions<TEntity, TSortKey> getOptions, CancellationToken cancellationToken = default) requires sort options when pagination is executed.");
            }

            await using var dbContext = this.ContextFactory.CreateDbContext();
            var queryable = dbContext.Set<TEntity>().AsQueryable();

            if (getOptions.FilterExpression != null)
                queryable = queryable.Where(getOptions.FilterExpression);

            if (getOptions.Sort?.SortExpression != null)
            {
                queryable = getOptions.Sort.Ascending
                    ? queryable.OrderBy(getOptions.Sort.SortExpression)
                    : queryable.OrderByDescending(getOptions.Sort.SortExpression);
            }

            if (getOptions.Pagination != null)
            {
                queryable = queryable
                    .Skip((getOptions.Pagination.Page - 1) * getOptions.Pagination.Count)
                    .Take(getOptions.Pagination.Count);
            }

            operationResult.Object = await queryable.ToListAsync(cancellationToken: cancellationToken);
            return operationResult;
        }

        public virtual async Task<IOperationResult<List<TProjection>>> GetAsync<TSortKey, TProjection>(
            IGetOptions<TEntity, TSortKey, TProjection> getOptions, 
            CancellationToken cancellationToken = default)
        {
            var operationResult = new OperationResult<List<TProjection>>();
            operationResult.ValidateNotNull(getOptions, nameof(EfCoreService<TEntity>), nameof(this.GetAsync), nameof(getOptions));

            if (!operationResult.Success)
                return operationResult;

            if (getOptions.Projection?.Selector == null)
            {
                operationResult.AddErrorMessage(
                    "GetAsync<TSortKey, TProjection>(IGetOptions<TEntity, TSortKey, TProjection> getOptions, CancellationToken cancellationToken = default) requires a projection selector.");
            }

            if (getOptions.Pagination != null && getOptions.Sort == null)
            {
                operationResult.AddErrorMessage(
                    "GetAsync<TSortKey, TProjection>(IGetOptions<TEntity, TSortKey, TProjection> getOptions, CancellationToken cancellationToken = default) requires sort options when pagination is executed.");
            }
            
            if (!operationResult.Success)
                return operationResult;

            await using var dbContext = this.ContextFactory.CreateDbContext();
            var queryable = dbContext.Set<TEntity>().AsQueryable();

            if (getOptions.FilterExpression != null)
                queryable = queryable.Where(getOptions.FilterExpression);

            if (getOptions.Sort?.SortExpression != null)
            {
                queryable = getOptions.Sort.Ascending 
                    ? queryable.OrderBy(getOptions.Sort.SortExpression)
                    : queryable.OrderByDescending(getOptions.Sort.SortExpression);
            }

            if (getOptions.Pagination != null)
            {
                queryable = queryable
                    .Skip((getOptions.Pagination.Page - 1) * getOptions.Pagination.Count)
                    .Take(getOptions.Pagination.Count);
            }

            operationResult.Object = await queryable.Select(getOptions.Projection.Selector).ToListAsync(cancellationToken: cancellationToken);
            return operationResult;
        }

        // protected async Task<List<TEntity>> FilterAsNoTrackingAsync(Expression<Func<TEntity, bool>> expressionFilter)
        // {
        //     if (expressionFilter is null)
        //         return default;
        //
        //     await using var dbContext = this.ContextFactory.CreateDbContext();
        //     return await dbContext.Set<TEntity>()
        //         .Where(expressionFilter)
        //         .ToListAsync();
        // }

        // protected static async Task<List<TEntity>> FilterTrackingAsync(Expression<Func<TEntity, bool>> expressionFilter, ApplicationDbContext dbContext)
        // {
        //     if (expressionFilter is null)
        //         return default;
        //
        //     if (dbContext == null)
        //         throw new ArgumentNullException(nameof(dbContext));
        //
        //     return await dbContext.Set<TEntity>()
        //         .Where(expressionFilter)
        //         .ToListAsync();
        // }

        /// <inheritdoc />
        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            await using var dbContext = this.ContextFactory.CreateDbContext();

            return await dbContext.Set<TEntity>()
                .CountAsync(cancellationToken: cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            await using var dbContext = this.ContextFactory.CreateDbContext();

            return await dbContext.Set<TEntity>()
                .CountAsync(filterExpression);
        }

        /// <inheritdoc />
        public virtual async Task<IOperationResult<TEntity>> UpdateAsync(IEnumerable<object> identifiers, TEntity entity, CancellationToken cancellationToken = default)
        {
            var operationResult = new OperationResult<TEntity>();
            operationResult.ValidateNotNull(entity, nameof(EfCoreService<TEntity>), nameof(this.UpdateAsync), nameof(entity));
                     
            if (!operationResult.Success)
                return operationResult;

            var getEntityOperationResult = await this.GetAsync(identifiers, cancellationToken);
            if (!getEntityOperationResult.Success)
            {
                operationResult.AddOperationResult(getEntityOperationResult);
                return operationResult;
            }
   
            this.ApplyAuditInformation(entity, getEntityOperationResult.Object);
            
            await using var dbContext = this.ContextFactory.CreateDbContext();
            dbContext.Set<TEntity>().Update(entity);
            var affectedRows = await dbContext.SaveChangesAsync(cancellationToken);

            ValidateAffectedRows(
                affectedRows,
                expected: 1, 
                nameof(EfCoreService<TEntity>), 
                nameof(this.UpdateAsync), 
                typeof(TEntity).FullName);

            operationResult.AffectedRows = affectedRows;
            operationResult.Object = entity;

            return operationResult;
        }

        public IOperationResult<TModel> ValidatePermissions<TModel>(TModel model, PermissionScope scope, Permissions permissions) where TModel : class
        {
            var operationResult = new OperationResult<TModel>();
            operationResult.ValidateNotNull(model, nameof(EfCoreService<TModel>), nameof(this.ValidatePermissions), nameof(model));

            var hasPermissions = this._authenticationService.HasPermissions(scope, permissions);
            operationResult.ValidatePermissions(
                hasPermissions,
                this._auditLogger.ConstructLogAction(hasPermissions, scope, permissions));
            
            return operationResult;
        }
        
        public IOperationResult<TModel> ValidatePermissions<TKey, TModel>(TModel model, PermissionScope scope, Permissions permissions, TKey entityId) where TModel : class
        {
            var operationResult = new OperationResult<TModel>();
            operationResult.ValidateNotNull(model, nameof(EfCoreService<TModel>), nameof(this.ValidatePermissions), nameof(model));
            
            var hasPermissions = this._authenticationService.HasPermissions(scope, permissions);
            operationResult.ValidatePermissions(
                hasPermissions,
                this._auditLogger.ConstructLogAction(hasPermissions, scope, permissions, entityId));
           
            return operationResult;
        }

        public IOperationResult<TModel> ValidatePermissions<TModel>(TModel model, PermissionScope scope, Permissions permissions, object[] identifiers)
            where TModel : class
        {
            var operationResult = new OperationResult<TModel>();
            operationResult.ValidateNotNull(model, nameof(EfCoreService<TModel>), nameof(this.ValidatePermissions), nameof(model));

            var hasPermissions = this._authenticationService.HasPermissions(scope, permissions);
            operationResult.ValidatePermissions(
                hasPermissions,
                this._auditLogger.ConstructLogAction(hasPermissions, scope, permissions, string.Join(", ", identifiers ?? Array.Empty<object>())));

            return operationResult;
        }

        public IOperationResult<T> ValidateUserIsAuthenticated<T>()
        {
            var operationResult = new OperationResult<T>();
            if (!this._authenticationService.Authenticated)
                operationResult.AddErrorMessage("User is not authenticated!");

            return operationResult;
        }
        
        public IOperationResult<T> ValidateUserIsAuthenticated<T>(IOperationResult<T> operationResult)
        {
            if (!this._authenticationService.Authenticated)
                operationResult.AddErrorMessage("User is not authenticated!");

            return operationResult;
        }

        private void ApplyAuditInformation(TEntity entity)
        {
            if (entity is IAuditInformation auditModel)
            {
                var newEntity = auditModel.Created == default;

                if (newEntity)
                {
                    auditModel.Created = DateTimeOffset.UtcNow;
                    auditModel.CreatedBy = this._authenticationService.Context?.Id ?? 0;
                }
                else
                {
                    auditModel.LastModified = DateTimeOffset.UtcNow;
                    auditModel.LastModifiedBy = this._authenticationService.Context?.Id ?? 0;
                }
            }
        }
        
        private void ApplyAuditInformation(TEntity entity, TEntity databaseEntity)
        {
            if (entity is IAuditInformation auditModel && databaseEntity is IAuditInformation databaseAuditModel)
            {
                var newEntity = databaseAuditModel.Created == default;

                if (newEntity)
                {
                    auditModel.Created = DateTimeOffset.UtcNow;
                    auditModel.CreatedBy = this._authenticationService.Context?.Id ?? 0;
                }
                else
                {
                    auditModel.Created = databaseAuditModel.Created;
                    auditModel.CreatedBy = databaseAuditModel.CreatedBy;
                    auditModel.LastModified = DateTimeOffset.UtcNow;
                    auditModel.LastModifiedBy = this._authenticationService.Context?.Id ?? 0;
                }
            }
        }
    }
}