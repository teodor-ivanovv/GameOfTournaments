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
    using Microsoft.EntityFrameworkCore;
    using static Data.Infrastructure.DatabaseIntegrity;
    
    public abstract class EfCoreService<TEntity> : IEfCoreService<TEntity> where TEntity : class
    {
        private readonly IAuthenticationService _authenticationService;

        protected IDbContextFactory<ApplicationDbContext> ContextFactory { get; private set; }

        protected EfCoreService(IDbContextFactory<ApplicationDbContext> contextFactory, IAuthenticationService authenticationService)
        {
            this.ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            this._authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
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
        public async Task<int> CreateManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await using var dbContext = this.ContextFactory.CreateDbContext();
            var enumerated = entities.ToList();
            await dbContext.AddRangeAsync(enumerated, cancellationToken);
            var writtenEntries = await dbContext.SaveChangesAsync(cancellationToken);

            ValidateAffectedRows(
                writtenEntries,
                expected: enumerated.Count, 
                nameof(EfCoreService<TEntity>), 
                nameof(this.CreateAsync), 
                typeof(TEntity).FullName);

            return writtenEntries;
        }

        /// <inheritdoc />
        public async Task<TEntity> GetAsync(IEnumerable<object> identifiers, CancellationToken cancellationToken = default)
        {
            var enumerated = identifiers as object[] ?? identifiers.ToArray();

            if (identifiers == null || !enumerated.Any())
                throw new ArgumentNullException(nameof(identifiers));

            await using var dbContext = this.ContextFactory.CreateDbContext();

            return await dbContext.Set<TEntity>()
                .FindAsync(enumerated.ToArray(), cancellationToken: cancellationToken)
                .AsTask();
        }

        /// <inheritdoc />
        public async Task<List<TEntity>> GetAsync(IGetOptions<TEntity> getOptions, CancellationToken cancellationToken = default)
        {
            if (getOptions == null)
                throw new ArgumentNullException(nameof(getOptions));

            await using var dbContext = this.ContextFactory.CreateDbContext();
            var queryable = dbContext.Set<TEntity>().AsQueryable();

            if (getOptions.FilterExpression != null)
                queryable = queryable.Where(getOptions.FilterExpression);

            return await queryable.ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<List<TEntity>> GetAsync<TSortKey>(IGetOptions<TEntity, TSortKey> getOptions, CancellationToken cancellationToken = default)
        {
            if (getOptions == null)
                throw new ArgumentNullException(nameof(getOptions));

            if (getOptions.Pagination != null && getOptions.Sort == null)
                throw new ArgumentNullException(
                    nameof(getOptions),
                    "GetAsync<TSortKey>(IGetOptions<TEntity, TSortKey> getOptions, CancellationToken cancellationToken = default) requires sort options when pagination is executed.");

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

            return await queryable.ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<List<TProjection>> GetAsync<TSortKey, TProjection>(
            IGetOptions<TEntity, TSortKey, TProjection> getOptions, 
            CancellationToken cancellationToken = default)
        {
            if (getOptions == null)
                throw new ArgumentNullException(nameof(getOptions));

            if (getOptions.Projection?.Selector == null)
                throw new ArgumentNullException(
                    nameof(getOptions),
                    "GetAsync<TSortKey, TProjection>(IGetOptions<TEntity, TSortKey, TProjection> getOptions, CancellationToken cancellationToken = default) requires a projection selector.");

            if (getOptions.Pagination != null && getOptions.Sort == null)
                throw new ArgumentNullException(
                    nameof(getOptions),
                    "GetAsync<TSortKey, TProjection>(IGetOptions<TEntity, TSortKey, TProjection> getOptions, CancellationToken cancellationToken = default) requires sort options when pagination is executed.");

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

            return await queryable.Select(getOptions.Projection.Selector).ToListAsync(cancellationToken: cancellationToken);
        }

        protected async Task<List<TEntity>> FilterAsNoTrackingAsync(Expression<Func<TEntity, bool>> expressionFilter)
        {
            if (expressionFilter is null)
                return default;

            await using var dbContext = this.ContextFactory.CreateDbContext();
            return await dbContext.Set<TEntity>()
                .Where(expressionFilter)
                .ToListAsync();
        }

        protected static async Task<List<TEntity>> FilterTrackingAsync(Expression<Func<TEntity, bool>> expressionFilter, ApplicationDbContext dbContext)
        {
            if (expressionFilter is null)
                return default;

            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            return await dbContext.Set<TEntity>()
                .Where(expressionFilter)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            await using var dbContext = this.ContextFactory.CreateDbContext();

            return await dbContext.Set<TEntity>()
                .CountAsync(cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public async Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await using var dbContext = this.ContextFactory.CreateDbContext();
            dbContext.Set<TEntity>().Update(entity);
            var affectedRows = await dbContext.SaveChangesAsync(cancellationToken);

            ValidateAffectedRows(
                affectedRows,
                expected: 1, 
                nameof(EfCoreService<TEntity>), 
                nameof(this.UpdateAsync), 
                typeof(TEntity).FullName);

            return affectedRows;
        }

        /// <inheritdoc />
        public async Task<int> DeleteAsync(IEnumerable<object> identifiers, CancellationToken cancellationToken = default)
        {
            var enumerated = identifiers as object[] ?? identifiers.ToArray();

            if (identifiers == null || !enumerated.Any())
                throw new ArgumentNullException(nameof(identifiers));

            var entity = await this.GetAsync(enumerated.ToArray(), cancellationToken);

            if (entity == null)
                return 0;

            await using var dbContext = this.ContextFactory.CreateDbContext();
            dbContext.Remove(entity);
            var affectedRows = await dbContext.SaveChangesAsync(cancellationToken);

            ValidateAffectedRows(
                affectedRows,
                expected: 1,
                nameof(EfCoreService<TEntity>),
                nameof(this.DeleteAsync), 
                typeof(TEntity).FullName);

            return affectedRows;
        }

        /// <inheritdoc />
        public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken = default)
        {
            if (filterExpression == null)
                throw new ArgumentNullException(nameof(filterExpression));

            await using var dbContext = this.ContextFactory.CreateDbContext();
            var entities = await this.GetAsync(new GetOptions<TEntity, string> { FilterExpression = filterExpression, }, cancellationToken);

            dbContext.RemoveRange(entities);
            var affectedRows = await dbContext.SaveChangesAsync(cancellationToken);

            ValidateAffectedRows(
                affectedRows, 
                expected: entities.Count, 
                nameof(EfCoreService<TEntity>),
                nameof(this.DeleteAsync), 
                typeof(TEntity).FullName);

            return affectedRows;
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
    }
}