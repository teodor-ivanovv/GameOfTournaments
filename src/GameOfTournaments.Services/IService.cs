namespace GameOfTournaments.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Services.Infrastructure;
    using GameOfTournaments.Shared;
    using Microsoft.EntityFrameworkCore;

    public interface IService<TEntity>
    {
        /// <summary>
        /// Creates/inserts the given <paramref name="entity"/> in the registered database provider.
        /// </summary>
        /// <param name="entity">The entity to insert in the database.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>A <see cref="Task"/> of <see cref="IOperationResult{T}"/> of <see cref="TEntity"/> representing the created entity.</returns>
        Task<IOperationResult<TEntity>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates/inserts the given <paramref name="entities"/> in the registered database provider.
        /// </summary>
        /// <param name="entities">The entities to insert in the database.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>A <see cref="Task{TResult}"/> of <see cref="IOperationResult{T}"/> of <see cref="IEnumerable{T}"/> of <see cref="TEntity"/> representing the created entities.</returns>
        Task<IOperationResult<IEnumerable<TEntity>>> CreateManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets/retrieves an entity of <typeparamref name="TEntity"/> by the given <see cref="IEnumerable{T}"/> of <see cref="object"/> identifiers.
        /// </summary>
        /// <param name="identifiers">The identifiers of the given entity to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>A <see cref="Task{TResult}"/> of <see cref="IOperationResult{T}"/> of <typeparamref name="TEntity"/> representing the retrieved entity or null if it doesn't exist in the database.</returns>
        Task<IOperationResult<TEntity>> GetAsync(IEnumerable<object> identifiers, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets/retrieves entities of type <typeparamref name="TEntity"/> by the given <paramref name="getOptions"/> options configuration.
        /// </summary>
        /// <param name="getOptions">The get configuration options used to specify filter, sorting, projection and pagination.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>A <see cref="Task{TResult}"/> of <see cref="IOperationResult{T}"/> of <see cref="List{T}"/> of <see cref="TEntity"/> entities.</returns>
        Task<IOperationResult<List<TEntity>>> GetAsync(IGetOptions<TEntity> getOptions, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets/retrieves entities of type <typeparamref name="TEntity"/> by the given <paramref name="getOptions"/> options configuration.
        /// </summary>
        /// <param name="getOptions">The get configuration options used to specify filter, sorting, projection and pagination.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be cancelled.</param>
        /// <typeparam name="TSortKey">The type of the property that will be used to sort the result entities.</typeparam>
        /// <returns>A <see cref="Task{TResult}"/> of <see cref="IOperationResult{T}"/> of <see cref="List{T}"/> of <see cref="TEntity"/> entities.</returns>
        Task<IOperationResult<List<TEntity>>> GetAsync<TSortKey>(IGetOptions<TEntity, TSortKey> getOptions, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets/retrieves entities of type <typeparamref name="TEntity"/> by the given <paramref name="getOptions"/> options configuration.
        /// </summary>
        /// <param name="getOptions">The get configuration options used to specify filter, sorting, projection and pagination.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be cancelled.</param>
        /// <typeparam name="TSortKey">The type of the property that will be used to sort the result entities.</typeparam>
        /// <typeparam name="TProjection">The type of the resulted projected entity that will be mapped from the original <typeparamref name="TEntity"/>.</typeparam>
        /// <returns>A <see cref="Task{TResult}"/> of <see cref="IOperationResult{T}"/> of <see cref="List{T}"/> of <see cref="TProjection"/> entities.</returns>
        Task<IOperationResult<List<TProjection>>> GetAsync<TSortKey, TProjection>(IGetOptions<TEntity, TSortKey, TProjection> getOptions, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the count of all entities represented in the given database table. 
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>A <see cref="Task{TResult}"/> of <see cref="int"/> representing the count of all entities represented in the given database table.</returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        
        Task<int> CountAsync(Expression<Func<TEntity, bool>> filterExpression);

        /// <summary>
        /// Updates the given <paramref name="entity"/> in the registered database provider. If primary keys have been changed this operation will throw a <see cref="DbUpdateConcurrencyException"/>.
        /// </summary>
        /// <param name="identifiers"></param>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be cancelled.</param>
        /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">Thrown when primary keys have been changed.</exception>
        /// <returns>A <see cref="Task{TResult}"/> of <see cref="IOperationResult{T}"/> <see cref="TEntity"/> representing the updated entity.</returns>
        Task<IOperationResult<TEntity>> UpdateAsync(IEnumerable<object> identifiers, TEntity entity, CancellationToken cancellationToken = default);

        IOperationResult<TModel> ValidatePermissions<TModel>(TModel model, PermissionScope scope, Permissions permissions) where TModel : class;

        IOperationResult<TModel> ValidatePermissions<TKey, TModel>(TModel model, PermissionScope scope, Permissions permissions, TKey entityId) where TModel : class;

        IOperationResult<TModel> ValidatePermissions<TModel>(TModel model, PermissionScope scope, Permissions permissions, object[] identifiers) where TModel : class;

        IOperationResult<T> ValidateUserIsAuthenticated<T>();
        
        IOperationResult<T> ValidateUserIsAuthenticated<T>(IOperationResult<T> operationResult);
    }
}