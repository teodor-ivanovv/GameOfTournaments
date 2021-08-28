namespace GameOfTournaments.Services.Infrastructure
{
    using System;
    using System.Linq.Expressions;

    public class GetOptions<TEntity> : IGetOptions<TEntity>
    {
        /// <inheritdoc />
        public Expression<Func<TEntity, bool>> FilterExpression { get; set; }
    }
  
    public class GetOptions<TEntity, TSortKey> : GetOptions<TEntity>, IGetOptions<TEntity, TSortKey>
    {
        /// <inheritdoc />
        public ISortOptions<TEntity, TSortKey> Sort { get; set; }

        /// <inheritdoc />
        public IPageOptions Pagination { get; set; }
    }
  
    public class GetOptions<TEntity, TSortKey, TResult> : GetOptions<TEntity, TSortKey>, IGetOptions<TEntity, TSortKey, TResult>
    {
        /// <inheritdoc />
        public IProjectionOptions<TEntity, TResult> Projection { get; set; }
    }
}