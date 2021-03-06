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

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<ApplicationUserAccount> ApplicationUsersAccounts { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<Log> Logs { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Bet> Bets { get; set; }

        public DbSet<News> News { get; set; }

        public DbSet<Tournament> Tournaments { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // TODO: Indexes
            // TODO: Move this to configurations

            base.OnModelCreating(builder);
            
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Permissions)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey(p => p.ApplicationUserId);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.ApplicationUserAccount)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey<ApplicationUserAccount>(a => a.ApplicationUserId);
        }
    }
}