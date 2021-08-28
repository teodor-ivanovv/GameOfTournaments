namespace GameOfTournaments.Services.Infrastructure
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents an interface, specifying sort options for the given <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that will be used when sorting.</typeparam>
    /// <typeparam name="TSortKey">The type of the property that will be used to sort on.</typeparam>
    public interface ISortOptions<TEntity, TSortKey>
    {
        /// <summary>
        /// Gets a property that indicates if the sort is in an ascending order.
        /// </summary>
        bool Ascending { get; }
    
        /// <summary>
        /// Gets the sort expression that is used when sorting entities of type <typeparamref name="TEntity"/>.
        /// </summary>
        Expression<Func<TEntity, TSortKey>> SortExpression { get; }
    }
}