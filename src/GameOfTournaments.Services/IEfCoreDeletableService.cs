namespace GameOfTournaments.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Infrastructure;

    public interface IEfCoreDeletableService<TEntity> : IEfCoreService<TEntity> where TEntity : class, IDeletable
    {
        /// <summary>
        /// Marks the given entity of type <see cref="TEntity"/> by the given <see cref="IEnumerable{T}"/> of <see cref="object"/> identifiers as soft deleted.
        /// </summary>
        /// <param name="identifiers">The identifiers of the entity to be marked as soft deleted.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>A <see cref="Task"/> of <see cref="IOperationResult"/> of the <see cref="TEntity"/> representing the soft deleted entity.</returns>
        Task<IOperationResult<TEntity>> SoftDeleteAsync(IEnumerable<object> identifiers, CancellationToken cancellationToken = default);

        // /// <summary>
        // /// Deletes all entities of type <see cref="TEntity"/> by the given <paramref name="filterExpression"/>.
        // /// </summary>
        // /// <param name="filterExpression">The filter expression used to filter entities to delete.</param>
        // /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be cancelled.</param>
        // /// <returns>A <see cref="Task{TResult}"/> of <see cref="int"/> representing the affected rows of the delete operation.</returns>
        // Task<int> SoftDeleteAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken = default);
    }
}