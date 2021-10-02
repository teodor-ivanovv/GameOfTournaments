namespace GameOfTournaments.Data.Models
{
    using System;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Shared;

    public class AuditLog : LogBase
    {
        public PermissionScope Scope { get; set; }

        public Permissions Permissions { get; set; }

        public DateTimeOffset ActionTime { get; set; }

        public bool HasPermissions { get; set; }

        public string EntityId { get; set; }
    }
}