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
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IJwtService jwtService;
        private readonly IApplicationUserCache applicationUserCache;
        private readonly IOptions<ApplicationSettings> options;

        public UsersController(
            IHttpContextAccessor httpContextAccessor, 
            IAuthenticationService authenticationService,
            UserManager<ApplicationUser> userManager, 
            IJwtService jwtService,
            IApplicationUserCache applicationUserCache,
            IOptions<ApplicationSettings> options)
            : base(httpContextAccessor, authenticationService, applicationUserCache)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            this.applicationUserCache = applicationUserCache ?? throw new ArgumentNullException(nameof(applicationUserCache));
            this.options = options ?? throw new ArgumentNullException(nameof(options));

            Guard.Against.Null(this.options.Value, nameof(this.options.Value));
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
            
            var result = await this.userManager.CreateAsync(user, registerUserModel.Password);
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
            
            var user = await this.userManager.FindByNameAsync(loginUserModel.Username);
            if (user == null)
            {
                operationResult.AddErrorMessage("User not found.");
                return this.Unauthorized(operationResult);
            }

            var passwordValid = await this.userManager.CheckPasswordAsync(user, loginUserModel.Password);
            if (!passwordValid)
            {
                operationResult.AddErrorMessage("Invalid username or password.");
                return this.Unauthorized(operationResult);
            }

            var token = this.jwtService.GenerateToken(user.Id.ToString(), user.UserName, this.options.Value.JwtSecret);
            operationResult.Object = new LoginUserResponseModel { Token = token };

            var roles = await this.userManager.GetRolesAsync(user);
            this.CacheApplicationUser(user, roles?.ToList());

            return this.Ok(operationResult);
        }

        private void CacheApplicationUser(ApplicationUser user, List<string> roles)
        {
            var applicationUserCacheModel = new ApplicationUserCacheModel
            {
                Id = user.Id,
                Roles = roles,
            };
            
            this.applicationUserCache.Cache(applicationUserCacheModel);
        }

        // CR: Remove this test method and implement integration tests
        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(GetFromCache))]
        public ActionResult GetFromCache(int id)
        {
            var cached = this.applicationUserCache.Get(id);
            
            if (cached == null)
                return this.BadRequest();

            return this.Ok(cached);
        }
    }
}