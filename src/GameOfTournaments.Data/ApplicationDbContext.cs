namespace GameOfTournaments.Data
{
    using GameOfTournaments.Data.Models;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Log> Logs { get; set; }
        
        public DbSet<AuditLog> AuditLogs { get; set; }
    }
}