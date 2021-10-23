namespace GameOfTournaments.Data.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using GameOfTournaments.Shared;

    public static class ExpressionBuilder
    {
        public static Expression<Func<T, bool>> JoinExpressions<T>(IEnumerable<Expression<Func<T, bool>>> expressions)
        {
            var enumerated = expressions.EnsureCollectionToList();
            if (!enumerated.Any())
                return t => true;
            
            if (enumerated.Count == 1)
                return enumerated.FirstOrDefault();

            var delegateType = typeof(Func<,>).GetGenericTypeDefinition().MakeGenericType(typeof(T), typeof(bool));
            var combined = enumerated.Cast<Expression>().Aggregate(Expression.AndAlso);

            return (Expression<Func<T, bool>>)Expression.Lambda(delegateType, combined);
        }
        
        public static Expression<Func<T, bool>> JoinExpressions<T>(params Expression<Func<T, bool>>[] expressions)
        {
            var enumerated = expressions.EnsureCollectionToList();
            if (!enumerated.Any())
                return t => true;

            if (enumerated.Count == 1)
                return enumerated.FirstOrDefault();

            var delegateType = typeof(Func<,>).GetGenericTypeDefinition().MakeGenericType(typeof(T), typeof(bool));
            var combined = enumerated.Cast<Expression>().Aggregate(Expression.AndAlso);

            return (Expression<Func<T, bool>>)Expression.Lambda(delegateType, combined);
        }
    }
}