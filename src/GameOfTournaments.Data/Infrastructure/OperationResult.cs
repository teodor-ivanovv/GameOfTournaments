namespace GameOfTournaments.Data.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using Ardalis.GuardClauses;
    using GameOfTournaments.Shared;

    public class OperationResult : OperationResult<object>
    {
    }

    public class OperationResult<T> : IOperationResult<T>
    {
        public bool Success { get; set; } = true;

        public int Code { get; set; }
        
        public int AffectedRows { get; set; }

        public T Object { get; set; }

        public List<string> Errors { get; set; } = new();

        public IOperationResult<T> AddErrorMessage(string errorMessage)
        {
            Guard.Against.NullOrWhiteSpace(errorMessage, nameof(errorMessage));
            this.Success = false;
            this.Errors.Add(errorMessage);

            return this;
        }

        public void AddOperationResult<TResult>(IOperationResult<TResult> operationResult)
        {
            foreach (var error in operationResult.Errors)
                this.AddErrorMessage(error);
        }

        public IOperationResult<T> ValidateNotNull(object obj, string className, string method, string parameter)
        {
            Guard.Against.NullOrWhiteSpace(className, nameof(className));
            Guard.Against.NullOrWhiteSpace(method, nameof(method));
            Guard.Against.NullOrWhiteSpace(parameter, nameof(parameter));

            if (obj == null)
                this.AddErrorMessage($"{nameof(this.ValidateNotNull)} - {className}.{method} - {parameter}.");

            return this;
        }

        public IOperationResult<T> ValidateNotWhiteSpace(string obj, string className, string method, string parameter)
        {
            Guard.Against.NullOrWhiteSpace(className, nameof(className));
            Guard.Against.NullOrWhiteSpace(method, nameof(method));
            Guard.Against.NullOrWhiteSpace(parameter, nameof(parameter));

            if (string.IsNullOrWhiteSpace(obj))
                this.AddErrorMessage($"{nameof(this.ValidateNotWhiteSpace)} - {className}.{method} - {parameter}.");

            return this;
        }

        public IOperationResult<T> ValidatePermissions(bool hasPermissions, Action auditLog)
        {
            if(!hasPermissions)
                this.AddErrorMessage("Current user does not have permissions for that action!");

            auditLog?.Invoke();

            return this;
        }

        public IOperationResult<T> SetCode(int code)
        {
            this.Code = code;

            return this;
        }

        public IOperationResult<TNewType> ChangeObjectType<TNewType>(TNewType newObject)
        {
            var operationResult = new OperationResult<TNewType>()
            {
                Code = this.Code,
                Object = newObject,
                Success = this.Success,
                AffectedRows = this.AffectedRows,
            };

            foreach (var error in this.Errors)
                operationResult.AddErrorMessage(error);

            return operationResult;
        }
    }
}