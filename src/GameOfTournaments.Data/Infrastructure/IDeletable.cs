namespace GameOfTournaments.Data.Infrastructure
{
    using System;

    public interface IDeletable
    {
        bool Deleted { get; set; }
        
        DateTimeOffset? Time { get; set; }
    }
}