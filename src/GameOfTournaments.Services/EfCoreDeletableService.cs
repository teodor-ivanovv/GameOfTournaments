namespace GameOfTournaments.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GameOfTournaments.Data;
    using GameOfTournaments.Data.Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using static Data.Infrastructure.DatabaseIntegrity;

    public class EfCoreDeletableService<TEntity> 
        : EfCoreService<TEntity>, IEfCoreDeletableService<TEntity> where TEntity : class, IDeletable
    {
        public EfCoreDeletableService(
            IDbContextFactory<ApplicationDbContext> contextFactory,
            IAuthenticationService authenticationService,
            IAuditLogger auditLogger)
            : base(contextFactory, authenticationService, auditLogger)
        {
        }
        
         /// <inheritdoc />
        public virtual async Task<IOperationResult<TEntity>> SoftDeleteAsync(IEnumerable<object> identifiers, CancellationToken cancellationToken = default)
        {
            var operationResult = new OperationResult<TEntity>();
            var enumerated = identifiers as object[] ?? identifiers.ToArray();
            if (!enumerated.Any())
                return new NotExistingOperationResult<TEntity>(typeof(TEntity).Name);

            var entity = await this.GetAsync(enumerated.ToArray(), cancellationToken);
            if (entity == null)
                return new NotExistingOperationResult<TEntity>(typeof(TEntity).Name);

            await using var dbContext = this.ContextFactory.CreateDbContext();
            entity.Deleted = true;
            entity.Time = DateTimeOffset.UtcNow;
       
            var affectedRows = await dbContext.SaveChangesAsync(cancellationToken);

            ValidateAffectedRows(
                affectedRows,
                expected: 1,
                nameof(EfCoreDeletableService<TEntity>),
                nameof(this.SoftDeleteAsync), 
                typeof(TEntity).FullName);

            operationResult.AffectedRows = affectedRows;
            operationResult.Object = entity;

            return operationResult;
        }

        // /// <inheritdoc />
        // public async Task<int> SoftDeleteAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken = default)
        // {
        //     if (filterExpression == null)
        //         throw new ArgumentNullException(nameof(filterExpression));
        //
        //     await using var dbContext = this.ContextFactory.CreateDbContext();
        //     var entities = await this.GetAsync(new GetOptions<TEntity, string> { FilterExpression = filterExpression, }, cancellationToken);
        //
        //     dbContext.RemoveRange(entities);
        //     var affectedRows = await dbContext.SaveChangesAsync(cancellationToken);
        //
        //     ValidateAffectedRows(
        //         affectedRows, 
        //         expected: entities.Count, 
        //         nameof(EfCoreService<TEntity>),
        //         nameof(this.SoftDeleteAsync), 
        //         typeof(TEntity).FullName);
        //
        //     return affectedRows;
        // }
    }
}