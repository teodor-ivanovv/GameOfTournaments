namespace GameOfTournaments.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services;
    using GameOfTournaments.Shared;
    using GameOfTournaments.Web.Cache.ApplicationUsers;
    using GameOfTournaments.Web.Factories;
    using GameOfTournaments.Web.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IApplicationUserCache _applicationUserCache;
        private readonly IOptions<ApplicationSettings> _options;

        public UsersController(
            IHttpContextAccessor httpContextAccessor, 
            IAuthenticationService authenticationService,
            UserManager<ApplicationUser> userManager, 
            IJwtService jwtService,
            IApplicationUserCache applicationUserCache,
            IOptions<ApplicationSettings> options)
            : base(httpContextAccessor, authenticationService, applicationUserCache)
        {
            this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this._jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            this._applicationUserCache = applicationUserCache ?? throw new ArgumentNullException(nameof(applicationUserCache));
            this._options = options ?? throw new ArgumentNullException(nameof(options));

            Guard.Against.Null(this._options.Value, nameof(this._options.Value));
        }
        
        // TODO: Add audit logging.
        // TODO: Move user manager related logic to authentication service.

        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(Register))]
        public async Task<ActionResult> Register(RegisterUserModel registerUserModel)
        {
            var operationResult = new OperationResult();
            operationResult.ValidateNotNull(registerUserModel, nameof(UsersController), nameof(this.Register), nameof(registerUserModel));

            if (!operationResult.Success)
                return this.BadRequest(operationResult);
            
            var user = ApplicationUserFactory.Create(registerUserModel);
            operationResult.ValidateNotNull(user, nameof(UsersController), nameof(this.Register), nameof(user));

            if (!operationResult.Success)
                return this.BadRequest(operationResult);
            
            var result = await this._userManager.CreateAsync(user, registerUserModel.Password);
            if (!result.Succeeded)
            {
                result.Errors.Select(e => operationResult.AddErrorMessage($"{e.Code} - {e.Description}"));
                return this.BadRequest(operationResult);
            }

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
            this.CacheApplicationUserAsync(user).ExecuteNonBlocking();

            return this.Ok(operationResult);
        }

        private async Task CacheApplicationUserAsync(ApplicationUser user)
        {
            var roles = await this._userManager.GetRolesAsync(user);
            var applicationUserCacheModel = new ApplicationUserCacheModel
            {
                Id = user.Id,
                Roles = roles?.ToList(),
            };
            
            this._applicationUserCache.Cache(applicationUserCacheModel);
        }

        // CR: Remove this test method and implement integration tests
        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(GetFromCache))]
        public ActionResult GetFromCache(int id)
        {
            var cached = this._applicationUserCache.Get(id);
            
            if (cached == null)
                return this.BadRequest();

            return this.Ok(cached);
        }
    }
}