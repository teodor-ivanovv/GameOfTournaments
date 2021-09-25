namespace GameOfTournaments.Data.Infrastructure
{
    public class NotExistingOperationResult : NotExistingOperationResult<object>
    {
        public NotExistingOperationResult(string entity)
            : base(entity)
        {
        }
    }
    
    public class NotExistingOperationResult<T> : OperationResult<T>
    {
        public NotExistingOperationResult(string entity)
        {
            this.AddErrorMessage($"{entity} does not exist!");
        }
    }
}