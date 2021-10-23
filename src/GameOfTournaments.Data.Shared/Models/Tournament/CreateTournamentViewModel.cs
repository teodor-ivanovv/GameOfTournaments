namespace GameOfTournaments.Data.Factories.Models.Tournament
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using GameOfTournaments.Data.Models;

    public class CreateTournamentViewModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal JoinFee { get; set; }

        public bool Public { get; set; }

        public decimal FirstPlacePrize { get; set; }

        public decimal SecondPlacePrize { get; set; }

        public decimal ThirdPlacePrize { get; set; }

        public int MaximumCompetitors { get; set; }

        public int? MinimumCompetitors { get; set; }

        public DateTimeOffset Start { get; set; }

        public DateTimeOffset End { get; set; }

        public Tournament ToTournament()
            => new()
            {
                Name = this.Name,
                Description = this.Description,
                JoinFee = this.JoinFee,
                Public = this.Public,
                FirstPlacePrize = this.FirstPlacePrize,
                SecondPlacePrize = this.SecondPlacePrize,
                ThirdPlacePrize = this.ThirdPlacePrize,
                MaximumCompetitors = this.MaximumCompetitors,
                MinimumCompetitors = this.MinimumCompetitors,
                Start = this.Start,
                End = this.End,
            };
    }
}