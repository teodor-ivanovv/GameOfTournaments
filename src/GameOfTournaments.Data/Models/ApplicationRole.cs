namespace GameOfTournaments.Data.Models
{
    using System;
    using GameOfTournaments.Data.Infrastructure;
    using Microsoft.AspNetCore.Identity;

    // TODO: Not used anymore
    public class ApplicationRole : IdentityRole<int>, IAuditInformation
    {
        public string Action { get; set; }
        
        public DateTimeOffset Created { get; set; }

        public int CreatedBy { get; set; }

        public DateTimeOffset? LastModified { get; set; }

        public int LastModifiedBy { get; set; }
    }
}