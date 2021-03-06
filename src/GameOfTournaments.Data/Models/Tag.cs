namespace GameOfTournaments.Data.Models
{
    using System;
    using GameOfTournaments.Data.Infrastructure;

    public class Tag : IIdentifiable<int>, IAuditInformation
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset Created { get; set; }

        public int CreatedBy { get; set; }

        public DateTimeOffset? LastModified { get; set; }

        public int LastModifiedBy { get; set; }
    }
}