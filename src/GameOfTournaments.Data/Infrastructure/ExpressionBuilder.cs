namespace GameOfTournaments.Data.Infrastructure
{
    using System;
    using System.Linq.Expressions;

    public static class ExpressionBuilder
    {
        public static Expression<Func<T, bool>> AndAlso<T>(
            this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            if (left == null && right == null)
                return default;

            if (left == null)
                return right;

            if (right == null)
                return left;
            
            var param = left.Parameters[0];
            if (ReferenceEquals(param, right.Parameters[0]))
            {
                return Expression.Lambda<Func<T, bool>>(
                    Expression.AndAlso(left.Body, right.Body), param);
            }
            
            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(
                    left.Body,
                    Expression.Invoke(right, param)), param);
        }
        
        public static Expression<Func<T, bool>> OrElse<T>(
            this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            if (left == null && right == null)
                return default;

            if (left == null)
                return right;

            if (right == null)
                return left;
            
            var param = left.Parameters[0];
            if (ReferenceEquals(param, right.Parameters[0]))
            {
                return Expression.Lambda<Func<T, bool>>(
                    Expression.OrElse(left.Body, right.Body), param);
            }
            
            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(
                    left.Body,
                    Expression.Invoke(right, param)), param);
        }
    }
}