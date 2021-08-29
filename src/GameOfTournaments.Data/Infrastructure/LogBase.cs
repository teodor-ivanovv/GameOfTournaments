namespace GameOfTournaments.Data.Infrastructure
{
    using System;

    public abstract class LogBase : IIdentifiable<int>
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public string IpAddress { get; set; }

        public int? UserId { get; set; }

        public DateTimeOffset Time { get; set; } = DateTimeOffset.UtcNow;
    }
}