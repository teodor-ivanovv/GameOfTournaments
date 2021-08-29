namespace GameOfTournaments.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services;
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
        private readonly IOptions<ApplicationSettings> options;

        public UsersController(
            IHttpContextAccessor httpContextAccessor, 
            IAuthenticationService authenticationService,
            UserManager<ApplicationUser> userManager, 
            IJwtService jwtService, 
            IOptions<ApplicationSettings> options)
            : base(httpContextAccessor, authenticationService)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
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

            return this.Ok(operationResult);
        }
    }
}