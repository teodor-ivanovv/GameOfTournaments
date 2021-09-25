namespace GameOfTournaments.Data.Models
{
    using System;
    using GameOfTournaments.Data.Infrastructure;

    public class AuditLog : LogBase
    {
        public string Action { get; set; }

        public DateTimeOffset ActionTime { get; set; }

        public bool HasPermissions { get; set; }

        public string EntityId { get; set; }
    }
}