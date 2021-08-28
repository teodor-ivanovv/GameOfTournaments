namespace GameOfTournaments.Data.Models
{
    using System;
    using System.Collections.Generic;
    using GameOfTournaments.Data.Infrastructure;

    public class News : IIdentifiable<int>, IAuditInformation
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTimeOffset Created { get; set; }

        public int CreatedBy { get; set; }

        public DateTimeOffset? LastModified { get; set; }

        public int LastModifiedBy { get; set; }

        public IEnumerable<Tag> Tags { get; set; } = new HashSet<Tag>();
    }
}