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
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
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
        protected IApplicationUserService ApplicationUserService { get; private set; }
        protected IApplicationUserAccountService ApplicationUserAccountService { get; private set; }
        protected IAuditLogger AuditLogger { get; private set; }
        protected IGameService GameService { get; private set; }
        protected ITournamentService TournamentService { get; private set; }

        #endregion
        
        protected BaseTests()
        {
            this.RegisterServiceProvider();
            this.AuthenticationService = this.ServiceProvider.GetRequiredService<IAuthenticationService>();
            this.AuditLogger = this.ServiceProvider.GetRequiredService<IAuditLogger>();
            this.ApplicationUserService = this.ServiceProvider.GetRequiredService<IApplicationUserService>();
            this.ApplicationUserAccountService = this.ServiceProvider.GetRequiredService<IApplicationUserAccountService>();
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
                .AddScoped<IApplicationUserService, ApplicationUserService>(s => new ApplicationUserService(MockUserManager(new List<ApplicationUser>()).Object))
                .AddScoped<IApplicationUserAccountService, ApplicationUserAccountService>()
                .AddScoped<IGameService, GameService>()
                .AddScoped<ITournamentService, TournamentService>()
                .AddScoped<IAuditLogger, AuditLogger>()
                .BuildServiceProvider();
        }
        
        public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

            return mgr;
        }
        
        // public static UserManager<TUser> TestUserManager<TUser>(IUserStore<TUser> store = null) where TUser : class
        // {
        //     store ??= new Mock<IUserStore<TUser>>().Object;
        //     var options = new Mock<IOptions<IdentityOptions>>();
        //     var idOptions = new IdentityOptions { Lockout = { AllowedForNewUsers = false } };
        //     options.Setup(o => o.Value).Returns(idOptions);
        //     var userValidators = new List<IUserValidator<TUser>>();
        //     var validator = new Mock<IUserValidator<TUser>>();
        //     userValidators.Add(validator.Object);
        //     var pwdValidators = new List<PasswordValidator<TUser>> { new() };
        //     var userManager = new UserManager<TUser>(
        //         store,
        //         options.Object,
        //         new PasswordHasher<TUser>(),
        //         userValidators,
        //         pwdValidators,
        //         new UpperInvariantLookupNormalizer(),
        //         new IdentityErrorDescriber(),
        //         null,
        //         new Mock<ILogger<UserManager<TUser>>>().Object);
        //
        //     validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>())).Returns(Task.FromResult(IdentityResult.Success)).Verifiable();
        //
        //     return userManager;
        // }
    }
}