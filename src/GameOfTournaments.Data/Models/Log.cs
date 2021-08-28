namespace GameOfTournaments.Data.Models
{
    using System;
    using GameOfTournaments.Data.Infrastructure;

    public class Log : IIdentifiable<int>
    {
        public int Id { get; set; }

        public LogSeverity Severity { get; set; }

        public string Message { get; set; }

        public string IpAddress { get; set; }

        public int? UserId { get; set; }

        public DateTimeOffset Time { get; set; } = DateTimeOffset.UtcNow;
    }
}