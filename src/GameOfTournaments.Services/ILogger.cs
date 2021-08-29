namespace GameOfTournaments.Services
{
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Data.Models;

    public interface ILogger : IEfCoreService<Log>
    {
        Task LogAsync(string message, LogSeverity severity);
        
        Task LogAsync(Log log);
    }
}