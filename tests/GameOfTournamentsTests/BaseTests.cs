namespace GameOfTournamentsTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GameOfTournaments.Data;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services;
    using GameOfTournaments.Services.Infrastructure;
    using GameOfTournaments.Shared;
    using KellermanSoftware.CompareNetObjects;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public abstract class BaseTests
    {
        private int _userId = 1;
        
        private readonly GetOptions<AuditLog, int> _auditLogGetOptions = new()
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
        protected ITournamentService TournamentService { get; private set; }

        #endregion
        
        protected BaseTests()
        {
            this.RegisterServiceProvider();
            this.AuthenticationService = this.ServiceProvider.GetRequiredService<IAuthenticationService>();
            this.AuditLogger = this.ServiceProvider.GetRequiredService<IAuditLogger>();
            this.GameService = this.ServiceProvider.GetRequiredService<IGameService>();
            this.TournamentService = this.ServiceProvider.GetRequiredService<ITournamentService>();
        }

        protected void AuthenticateUser()
        {
            this.AuthenticationService.Set(new AuthenticationContext(true, this._userId));
            this._userId++;
        }
        
        protected void AuthenticateUser(int id, params PermissionModel[] roles)
        {
            var authenticationContext = new AuthenticationContext(true, this._userId)
            {
                Id = id,
            };
            authenticationContext.Permissions.AddRange(roles);
            
            this._userId = id;
            this.AuthenticationService.Set(authenticationContext);
        }
        
        protected int AuthenticateUser(params PermissionModel[] roles)
        {
            var authenticationContext = new AuthenticationContext(true, this._userId)
            {
                Id = this._userId,
            };
            authenticationContext.Permissions.AddRange(roles);

            this.AuthenticationService.Set(authenticationContext);
            return this._userId++;
        }
        
        protected void AuthenticateUser(IAuthenticationContext authenticationContext)
        {
            if (authenticationContext != null)
                authenticationContext.Id = this._userId;
            
            this.AuthenticationService.Set(authenticationContext);
            
            this._userId++;
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
        
        protected void AssertOperationResult<T>(IOperationResult<T> operationResult, bool success = true)
        {
            Assert.NotNull(operationResult);
            Assert.Equal(success, operationResult.Success);

            if (success)
            {
                Assert.Empty(operationResult.Errors);
                Assert.NotNull(operationResult.Object);
            }
            else
            {
                Assert.NotEmpty(operationResult.Errors);
                Assert.Null(operationResult.Object);
            }
        }
        
        protected void AssertOperationResult<T>(IOperationResult<IEnumerable<T>> operationResult, int entities, bool success = true)
        {
            Assert.NotNull(operationResult);
            Assert.Equal(success, operationResult.Success);
            
            if (success)
            {
                Assert.Empty(operationResult.Errors);
                Assert.NotNull(operationResult.Object);
                Assert.Equal(entities, operationResult.Object.Count());
                Assert.Equal(entities, operationResult.Object.Count());
            }
            else
            {
                Assert.NotEmpty(operationResult.Errors);
                Assert.Null(operationResult.Object);
            }
        }

        protected void RegisterServiceProvider()
        {
            this.ServiceProvider = new ServiceCollection()
                .AddDbContextFactory<ApplicationDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddScoped<IAuthenticationService, AuthenticationService>()
                .AddScoped<IApplicationUserService, ApplicationUserService>()
                .AddScoped<IApplicationUserAccountService, ApplicationUserAccountService>()
                .AddScoped<IGameService, GameService>()
                .AddScoped<ITournamentService, TournamentService>()
                .AddScoped<IAuditLogger, AuditLogger>()
                .BuildServiceProvider();
        }
    }
}