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
       
        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<Log> Logs { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Bet> Bets { get; set; }

        public DbSet<News> News { get; set; }

        public DbSet<Tournament> Tournaments { get; set; }
    }
}