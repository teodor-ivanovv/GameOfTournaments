namespace GameOfTournaments.Shared
{
    using System;
    using System.Threading.Tasks;

    public static class TaskExtensions
    {
#pragma warning disable S3168
        public static async void ExecuteNonBlocking(this Task task)
#pragma warning restore S3168
        {
            try
            {
                if (task != null)
                    await task.ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                // TODO: Log
            }
        }
        
#pragma warning disable S3168
        public static async void ExecuteNonBlocking<T>(this Task<T> task)
#pragma warning restore S3168
        {
            try
            {
                if (task != null)
                    await task.ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                // TODO: Log
            }
        }
    }
}