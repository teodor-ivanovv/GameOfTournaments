namespace GameOfTournaments.Services
{
    using GameOfTournaments.Data;
    using GameOfTournaments.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class NewsService : EfCoreService<News>, INewsService
    {
        public NewsService(IDbContextFactory<ApplicationDbContext> contextFactory)
            : base(contextFactory)
        {
        }
    }
}