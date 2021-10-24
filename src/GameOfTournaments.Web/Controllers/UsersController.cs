namespace GameOfTournaments.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using GameOfTournaments.Data.Factories.Models;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services;
    using GameOfTournaments.Services.Infrastructure;
    using GameOfTournaments.Shared;
    using GameOfTournaments.Web.Cache.ApplicationUsers;
    using GameOfTournaments.Web.Factories;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IApplicationUserAccountService _applicationUserAccountService;
        private readonly IJwtService _jwtService;
        private readonly IApplicationUserCache _applicationUserCache;
        private readonly IPermissionService _permissionService;
        private readonly IOptions<ApplicationSettings> _options;

        public UsersController(
            IHttpContextAccessor httpContextAccessor, 
            IAuthenticationService authenticationService,
            IApplicationUserService applicationUserService,
            IApplicationUserAccountService applicationUserAccountService,
            UserManager<ApplicationUser> userManager, 
            IJwtService jwtService,
            IApplicationUserCache applicationUserCache,
            IPermissionService permissionService,
            IOptions<ApplicationSettings> options)
            : base(httpContextAccessor, authenticationService, applicationUserCache)
        {
            this._applicationUserService = applicationUserService;
            this._applicationUserAccountService = applicationUserAccountService;
            this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this._jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            this._applicationUserCache = applicationUserCache ?? throw new ArgumentNullException(nameof(applicationUserCache));
            this._permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
            this._options = options ?? throw new ArgumentNullException(nameof(options));

            Guard.Against.Null(this._options.Value, nameof(this._options.Value));
        }
        
        // TODO: Add audit logging.
        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(Register))]
        public async Task<ActionResult> Register(RegisterUserModel registerUserModel)
        {
            var operationResult = new OperationResult();
            operationResult.ValidateNotNull(registerUserModel, nameof(UsersController), nameof(this.Register), nameof(registerUserModel));
            if (!operationResult.Success)
                return this.BadRequest(operationResult);
            
            var createApplicationUserOperationResult = await this._applicationUserService.CreateAsync(registerUserModel);
            if (!createApplicationUserOperationResult.Success)
                return this.BadRequest(createApplicationUserOperationResult);

            var createApplicationUserAccountOperationResult = await this._applicationUserAccountService.CreateAsync(
                new ApplicationUserAccount { ApplicationUserId = createApplicationUserOperationResult.Object.Id, CreateTournamentsPerDay = 10, });
            if (!createApplicationUserAccountOperationResult.Success)
                return this.BadRequest(createApplicationUserAccountOperationResult);

            return this.Ok();
        }
        
        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(Login))]
        public async Task<ActionResult<IOperationResult<LoginUserResponseModel>>> Login(LoginUserModel loginUserModel)
        {
            var operationResult = new OperationResult<LoginUserResponseModel>();
            operationResult.ValidateNotNull(loginUserModel, nameof(UsersController), nameof(this.Login), nameof(loginUserModel));

            if (!operationResult.Success)
                return this.BadRequest(operationResult);
            
            var user = await this._userManager.FindByNameAsync(loginUserModel.Username);
            if (user == null)
            {
                operationResult.AddErrorMessage("User not found.");
                return this.Unauthorized(operationResult);
            }

            var passwordValid = await this._userManager.CheckPasswordAsync(user, loginUserModel.Password);
            if (!passwordValid)
            {
                operationResult.AddErrorMessage("Invalid username or password.");
                return this.Unauthorized(operationResult);
            }

            var token = this._jwtService.GenerateToken(user.Id.ToString(), user.UserName, this._options.Value.JwtSecret);
            operationResult.Object = new LoginUserResponseModel { Token = token };

            // No need to slow the login request.
            this.CacheApplicationUserAsync(user)
                .ExecuteNonBlocking();

            return this.Ok(operationResult);
        }

        private async Task CacheApplicationUserAsync(ApplicationUser user)
        {
            var permissionsOperationResult = await this._permissionService.GetAsync(
                new GetOptions<Permission, int, PermissionModel>
                {
                    FilterExpression = p => p.ApplicationUserId == user.Id,
                    Projection = new ProjectionOptions<Permission, PermissionModel>(
                        p => new PermissionModel
                        {
                            Scope = p.Scope,
                            Permissions = p.Permissions,
                        }),
                });
            
            if (!permissionsOperationResult.Success)
                return;

            var applicationUserCacheModel = new ApplicationUserCacheModel
            {
                Id = user.Id,
                Permissions = permissionsOperationResult.Object?.ToList(),
            };
            
            this._applicationUserCache.Cache(applicationUserCacheModel);
        }
    }
}