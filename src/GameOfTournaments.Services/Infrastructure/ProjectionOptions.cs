namespace GameOfTournaments.Services.Infrastructure
{
    using System;
    using System.Linq.Expressions;

    public class ProjectionOptions<TSource, TResult> : IProjectionOptions<TSource, TResult>
    {
        public ProjectionOptions(Expression<Func<TSource, TResult>> selector)
        {
            this.Selector = selector ?? throw new ArgumentNullException(nameof(selector));
        }

        /// <inheritdoc />
        public Expression<Func<TSource, TResult>> Selector { get; }
    }
}