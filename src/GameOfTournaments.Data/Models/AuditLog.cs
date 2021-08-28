namespace GameOfTournaments.Data.Models
{
    using GameOfTournaments.Data.Infrastructure;

    public class AuditLog : LogBase
    {
        public string Action { get; set; }
    }
}