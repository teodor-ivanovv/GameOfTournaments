namespace GameOfTournaments.Data.Infrastructure
{
    using System.Collections.Generic;

    public interface IOperationResult : IOperationResult<object>
    {
    }

    public interface IOperationResult<out T>
    {
        bool Success { get; }
        
        int Code { get; }
        
        T Object { get; }
        
        IEnumerable<string> Errors { get; }

        IOperationResult<T> AddError(string errorMessage);

        IOperationResult<T> ValidateNotNull(object obj, string className, string method, string parameter);
        
        IOperationResult<T> ValidateNotWhiteSpace(string obj, string className, string method, string parameter);

        IOperationResult<T> SetCode(int code);
    }
}