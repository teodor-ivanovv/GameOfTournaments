namespace GameOfTournaments.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using GameOfTournaments.Data.Infrastructure;
    using static Shared.GameConstants;

    public class Game : IIdentifiable<int>, IAuditInformation, IDeletable
    {
        /// <inheritdoc/>
        public int Id { get; set; }

        [Required]
        [MinLength(NameMinimumLength)]
        [MaxLength(NameMaximumLength)]
        public string Name { get; set; }

        [Required]
        [MinLength(DescriptionMinimumLength)]
        [MaxLength(DescriptionMaximumLength)]
        public string Description { get; set; }

        public DateTimeOffset Created { get; set; }

        public int CreatedBy { get; set; }

        public DateTimeOffset? LastModified { get; set; }

        public int LastModifiedBy { get; set; }

        public bool Deleted { get; set; }

        public DateTimeOffset? Time { get; set; }
    }
}