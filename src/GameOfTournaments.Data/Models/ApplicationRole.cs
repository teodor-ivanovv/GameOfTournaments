namespace GameOfTournaments.Data.Models
{
    using System;
    using GameOfTournaments.Data.Infrastructure;
    using Microsoft.AspNetCore.Identity;

    public class ApplicationRole : IdentityRole<int>, IAuditInformation
    {
        public string Action { get; set; }
        
        public DateTimeOffset Created { get; set; }

        public int CreatedBy { get; set; }

        public DateTimeOffset? LastModified { get; set; }

        public int LastModifiedBy { get; set; }
    }
}