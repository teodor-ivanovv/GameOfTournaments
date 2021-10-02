namespace GameOfTournaments.Data.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using GameOfTournaments.Shared;

    public interface IOperationResult : IOperationResult<object>
    {
    }

    public interface IOperationResult<out T>
    {
        bool Success { get; }
        
        int Code { get; }
        
        int AffectedRows { get; }
        
        T Object { get; }
        
        IEnumerable<string> Errors { get; }

        IOperationResult<T> AddErrorMessage(string errorMessage);

        IOperationResult<T> ValidateNotNull(object obj, string className, string method, string parameter);
        
        IOperationResult<T> ValidateNotWhiteSpace(string obj, string className, string method, string parameter);

        IOperationResult<T> ValidatePermissions(bool hasPermissions, Action auditLog);

        IOperationResult<T> SetCode(int code);

        IOperationResult<TNewType> ChangeObjectType<TNewType>(TNewType newObject);
    }
}