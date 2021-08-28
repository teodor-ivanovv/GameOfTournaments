namespace GameOfTournaments.Data.Infrastructure
{
    using System.Collections.Generic;
    using Ardalis.GuardClauses;

    public class OperationResult : OperationResult<object>
    {
    }

    public class OperationResult<T> : IOperationResult<T>
    {
        private readonly List<string> errors = new();
        
        public bool Success { get; private set; } = true;

        public int Code { get; private set; }

        public T Object { get; private set; }

        public IEnumerable<string> Errors => this.errors;

        public IOperationResult<T> AddError(string errorMessage)
        {
            Guard.Against.NullOrWhiteSpace(errorMessage, nameof(errorMessage));
            this.Success = false;
            this.errors.Add(errorMessage);

            return this;
        }

        public IOperationResult<T> ValidateNotNull(object obj, string className, string method, string parameter)
        {
            Guard.Against.NullOrWhiteSpace(className, nameof(className));
            Guard.Against.NullOrWhiteSpace(method, nameof(method));
            Guard.Against.NullOrWhiteSpace(parameter, nameof(parameter));

            if (obj == null)
                this.AddError($"{nameof(this.ValidateNotNull)} - {className}.{method} - {parameter}.");

            return this;
        }

        public IOperationResult<T> ValidateNotWhiteSpace(string obj, string className, string method, string parameter)
        {
            Guard.Against.NullOrWhiteSpace(className, nameof(className));
            Guard.Against.NullOrWhiteSpace(method, nameof(method));
            Guard.Against.NullOrWhiteSpace(parameter, nameof(parameter));

            if (string.IsNullOrWhiteSpace(obj))
                this.AddError($"{nameof(this.ValidateNotWhiteSpace)} - {className}.{method} - {parameter}.");

            return this;
        }

        public IOperationResult<T> SetCode(int code)
        {
            this.Code = code;

            return this;
        }
    }
}