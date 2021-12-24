namespace GameOfTournaments.Data.Models
{
    using System;
    using GameOfTournaments.Data.Infrastructure;

    public class Bet : IIdentifiable<int>, IAuditInformation
    {
        public int Id { get; set; }

        public int TournamentId { get; set; }
        
        public Tournament Tournament { get; set; }

        public int CompetitorId { get; set; }
        
        public ApplicationUser Competitor { get; set; }

        public decimal BetValue { get; set; }
        
        public decimal Fee { get; set; }
        
        public decimal Coefficient { get; set; }

        public DateTimeOffset Created { get; set; }

        public int CreatedBy { get; set; }

        public DateTimeOffset? LastModified { get; set; }

        public int LastModifiedBy { get; set; }
    }
}