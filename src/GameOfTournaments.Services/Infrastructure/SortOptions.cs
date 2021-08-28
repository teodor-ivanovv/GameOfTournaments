namespace GameOfTournaments.Services.Infrastructure
{
    using System;
    using System.Linq.Expressions;

    public class SortOptions<TEntity, TSortKey> : ISortOptions<TEntity, TSortKey>
    {
        public SortOptions(bool @ascending, Expression<Func<TEntity, TSortKey>> sortExpression)
        {
            this.Ascending = @ascending;
            this.SortExpression = sortExpression ?? throw new ArgumentNullException(nameof(sortExpression));
        }

        /// <inheritdoc />
        public bool Ascending { get; }

        /// <inheritdoc />
        public Expression<Func<TEntity, TSortKey>> SortExpression { get; }
    }
}