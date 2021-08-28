namespace GameOfTournaments.Services.Infrastructure
{
    using System;
    using System.Linq.Expressions;

    public interface IGetOptions<TEntity>
    {
        Expression<Func<TEntity, bool>> FilterExpression { get; set; }
    }

    public interface IGetOptions<TEntity, TSortKey> : IGetOptions<TEntity>
    {
        IPageOptions Pagination { get; set; }

        ISortOptions<TEntity, TSortKey> Sort { get; set; }
    }

    public interface IGetOptions<TEntity, TSortKey, TResult> : IGetOptions<TEntity, TSortKey>
    {
        IProjectionOptions<TEntity, TResult> Projection { get; set; }
    }
}