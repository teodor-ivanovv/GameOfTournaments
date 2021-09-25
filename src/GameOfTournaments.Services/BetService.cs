namespace GameOfTournaments.Services
{
    using GameOfTournaments.Data;
    using GameOfTournaments.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class BetService : EfCoreService<Bet>, IBetService
    {
        public BetService(IDbContextFactory<ApplicationDbContext> contextFactory, IAuthenticationService authenticationService)
            : base(contextFactory, authenticationService)
        {
        }
    }
}