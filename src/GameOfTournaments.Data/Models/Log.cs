namespace GameOfTournaments.Data.Models
{
    using GameOfTournaments.Data.Infrastructure;

    public class Log : LogBase
    {
        public Log()
        {
        }
        
        public Log(string message, LogSeverity severity)
        {
            this.Message = message;
            this.Severity = severity;
        }

        public LogSeverity Severity { get; set; }
    }
}