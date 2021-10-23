namespace GameOfTournaments.Data.Factories.Models.Tournament
{
    using System;
    using GameOfTournaments.Data.Infrastructure;

    public class TournamentViewModel : CreateTournamentViewModel
    {
        public int Id { get; set; }

        public TournamentStatus Status { get; set; }

        public DateTimeOffset Created { get; set; }
    }
}