namespace GameOfTournaments.Shared
{
    using System;
    using System.Threading.Tasks;

    public static class TaskExtensions
    {
        public static async void ExecuteNonBlocking(this Task task)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                // TODO: Log
            }
        }
    }
}