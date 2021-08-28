namespace GameOfTournaments.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Web.Factories;
    using GameOfTournaments.Web.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

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
                result.Errors.Select(e => operationResult.AddError($"{e.Code} - {e.Description}"));
                return this.BadRequest(operationResult);
            }

            return this.Ok();
        }
    }
}