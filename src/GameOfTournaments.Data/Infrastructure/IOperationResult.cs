namespace GameOfTournaments.Data.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using GameOfTournaments.Shared;

    public interface IOperationResult : IOperationResult<object>
    {
    }

    public interface IOperationResult<T>
    {
        bool Success { get; set; }
        
        int Code { get; set; }
        
        int AffectedRows { get;  set;}
        
        T Object { get; set; }
        
        List<string> Errors { get; set; }

        IOperationResult<T> AddErrorMessage(string errorMessage);
        
        void AddOperationResult<TResult>(IOperationResult<TResult> operationResult);

        IOperationResult<T> ValidateNotNull(object obj, string className, string method, string parameter);
        
        IOperationResult<T> ValidateNotWhiteSpace(string obj, string className, string method, string parameter);

        IOperationResult<T> ValidatePermissions(bool hasPermissions, Action auditLog);

        IOperationResult<T> SetCode(int code);

        IOperationResult<TNewType> ChangeObjectType<TNewType>(TNewType newObject);
    }
}