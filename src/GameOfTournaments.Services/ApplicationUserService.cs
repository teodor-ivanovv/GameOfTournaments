namespace GameOfTournaments.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Factories;
    using GameOfTournaments.Data.Factories.Models;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Data.Models;
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUserService : IApplicationUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUserService(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<IOperationResult<ApplicationUser>> CreateAsync(RegisterUserModel registerUserModel)
        {
            var operationResult = new OperationResult<ApplicationUser>();
            operationResult.ValidateNotNull(registerUserModel, nameof(ApplicationUserService), nameof(this.CreateAsync), nameof(registerUserModel));
            if (!operationResult.Success)
                return operationResult;
            
            var user = ApplicationUserFactory.Create(registerUserModel);
            operationResult.ValidateNotNull(user, nameof(ApplicationUserService), nameof(this.CreateAsync), nameof(user));
            if (!operationResult.Success)
                return operationResult;
            
            var result = await this._userManager.CreateAsync(user, registerUserModel.Password);
            foreach (var error in result.Errors)
                operationResult.AddErrorMessage(error.Description);

            operationResult.Object = user;
            return operationResult;
        }
    }
}