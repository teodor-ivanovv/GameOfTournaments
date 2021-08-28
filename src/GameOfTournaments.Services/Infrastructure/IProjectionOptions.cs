namespace GameOfTournaments.Services.Infrastructure
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents an interface, specifying projection options for the given <typeparamref name="TSource"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source entity that will be used to transform/map to <typeparamref name="TResult"/>.</typeparam>
    /// <typeparam name="TResult">The type of the result after the projection has been applied.</typeparam>
    public interface IProjectionOptions<TSource, TResult>
    {
        /// <summary>
        /// Gets the expression projection selector used to project properties for the given <typeparamref name="TSource"/>.
        /// </summary>
        Expression<Func<TSource, TResult>> Selector { get; }
    }
}