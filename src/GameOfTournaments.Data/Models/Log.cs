﻿namespace GameOfTournaments.Data.Models
{
    using GameOfTournaments.Data.Infrastructure;

    public class Log : LogBase
    {
        public LogSeverity Severity { get; set; }
    }
}