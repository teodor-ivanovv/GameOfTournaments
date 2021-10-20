namespace GameOfTournaments.Data.Models
{
    using System;
    using System.Collections.Generic;
    using GameOfTournaments.Data.Infrastructure;

    public class Tournament : IIdentifiable<int>, IAuditInformation
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal JoinFee { get; set; }

        public bool Public { get; set; }

        public decimal FirstPlacePrize { get; set; }

        public decimal SecondPlacePrize { get; set; }

        public decimal ThirdPlacePrize { get; set; }

        public int MaximumCompetitors { get; set; }
        
        public int? MinimumCompetitors { get; set; }

        public TournamentStatus Status { get; set; }

        public DateTimeOffset Start { get; set; }
        
        public DateTimeOffset End { get; set; }

        public DateTimeOffset Created { get; set; }

        public int CreatedBy { get; set; }

        public DateTimeOffset? LastModified { get; set; }

        public int LastModifiedBy { get; set; }

        public IEnumerable<ApplicationUser> Competitors { get; set; } = new HashSet<ApplicationUser>();
    }
}