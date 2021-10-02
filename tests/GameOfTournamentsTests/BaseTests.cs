namespace GameOfTournamentsTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GameOfTournaments.Data;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services;
    using GameOfTournaments.Services.Infrastructure;
    using KellermanSoftware.CompareNetObjects;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public abstract class BaseTests
    {
        private readonly GetOptions<AuditLog, int> auditLogGetOptions = new()
        {
            Sort = new SortOptions<AuditLog, int>(true, l => l.Id),
            Pagination = new PageOptions(1, 1000),
        };
        
        protected Random Random { get; } = new();

        protected IServiceProvider ServiceProvider { get; private set; }

        #region Services

        protected IAuthenticationService AuthenticationService { get; private set; }
        protected IAuditLogger AuditLogger { get; private set; }
        protected IGameService GameService { get; private set; }

        #endregion
        
        protected BaseTests()
        {
            this.RegisterServiceProvider();
            this.AuthenticationService = this.ServiceProvider.GetRequiredService<IAuthenticationService>();
            this.AuditLogger = this.ServiceProvider.GetRequiredService<IAuditLogger>();
            this.GameService = this.ServiceProvider.GetRequiredService<IGameService>();
        }

        protected void AuthenticateUser()
        {
            this.AuthenticationService.Set(new AuthenticationContext(true));
        }
        
        // protected void AuthenticateUser(params string[] roles)
        // {
        //     var authenticationContext = new AuthenticationContext(true);
        //     authenticationContext.Permissions.AddRange(roles);
        //     this.AuthenticationService.Set(authenticationContext);
        // }
        
        protected void AuthenticateUser(IAuthenticationContext authenticationContext)
        {
            this.AuthenticationService.Set(authenticationContext);
        }

        // protected async Task AssertAuditLogAsync(string action, int? userId, bool hasPermissions)
        // {
        //     var auditLogs = await this.AuditLogger.GetAsync(this.auditLogGetOptions);
        //     Assert.NotNull(auditLogs);
        //     Assert.Contains(
        //         auditLogs, 
        //         log => log.Action == action && log.UserId == userId && log.HasPermissions == hasPermissions);
        // }
        
        protected static bool DeepEqual(object a, object b)
        {
            var compareLogic = new CompareLogic();
            var result = compareLogic.Compare(a, b);
            return result.AreEqual;
        }

        protected void RegisterServiceProvider()
        {
            this.ServiceProvider = new ServiceCollection()
                .AddDbContextFactory<ApplicationDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddScoped<IAuthenticationService, AuthenticationService>()
                .AddScoped<IGameService, GameService>()
                .AddScoped<IAuditLogger, AuditLogger>()
                .BuildServiceProvider();
        }
    }
}