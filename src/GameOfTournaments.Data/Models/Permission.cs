namespace GameOfTournaments.Data.Models
{
    using System;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Shared;

    public class Permission : IIdentifiable<int>, IAuditInformation, IDeletable
    {
        public int Id { get; set; }
        
        public ApplicationUser ApplicationUser { get; set; }
        
        public int ApplicationUserId { get; set; }
        
        public Permissions Permissions { get; set; }
        
        public PermissionScope Scope { get; set; }

        // Audit information
        public DateTimeOffset Created { get; set; }

        public int CreatedBy { get; set; }

        public DateTimeOffset? LastModified { get; set; }

        public int LastModifiedBy { get; set; }

        // Deleted information
        public bool Deleted { get; set; }

        public DateTimeOffset? Time { get; set; }
    }
}