namespace GameOfTournaments.Services
{
    using GameOfTournaments.Data;
    using GameOfTournaments.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationUserAccountService : EfCoreService<ApplicationUserAccount>, IApplicationUserAccountService
    {
        public ApplicationUserAccountService(IDbContextFactory<ApplicationDbContext> contextFactory, IAuthenticationService authenticationService, IAuditLogger auditLogger)
            : base(contextFactory, authenticationService, auditLogger)
        {
        }
    }
}