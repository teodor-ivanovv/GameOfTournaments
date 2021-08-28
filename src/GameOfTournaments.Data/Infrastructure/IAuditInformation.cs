namespace GameOfTournaments.Data.Infrastructure
{
    using System;

    public interface IAuditInformation
    {
        public DateTimeOffset Created { get; set; }
        
        public int CreatedBy { get; set; }
        
        public DateTimeOffset? LastModified { get; set; }
        
        public int LastModifiedBy { get; set; }
        
        // Can introduce more detailed history.
    }
}