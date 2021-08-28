namespace GameOfTournaments.Services
{
    using GameOfTournaments.Data;
    using GameOfTournaments.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class TournamentService : EfCoreService<Tournament>, ITournamentService
    {
        public TournamentService(IDbContextFactory<ApplicationDbContext> contextFactory)
            : base(contextFactory)
        {
        }
    }
}