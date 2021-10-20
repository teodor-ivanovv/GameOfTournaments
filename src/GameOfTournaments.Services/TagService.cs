namespace GameOfTournaments.Services
{
    using GameOfTournaments.Data;
    using GameOfTournaments.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class TagService : EfCoreService<Tag>, ITagService
    {
        public TagService(IDbContextFactory<ApplicationDbContext> contextFactory, IAuthenticationService authenticationService, IAuditLogger auditLogger)
            : base(contextFactory, authenticationService, auditLogger)
        {
        }
    }
}