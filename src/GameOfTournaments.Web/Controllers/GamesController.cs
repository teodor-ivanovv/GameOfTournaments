namespace GameOfTournaments.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Factories.Models.Game;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services;
    using GameOfTournaments.Services.Infrastructure;
    using GameOfTournaments.Web.Cache.ApplicationUsers;
    using GameOfTournaments.Web.Factories;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class GamesController : Controller
    {
        private readonly IGameService gameService;

        public GamesController(
            IHttpContextAccessor httpContextAccessor, 
            IAuthenticationService authenticationService,
            IGameService gameService,
            IApplicationUserCache applicationUserCache)
            : base(httpContextAccessor, authenticationService, applicationUserCache)
        {
            this.gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameViewModel>>> Get()
        {
            if (!this.AuthenticationService.Authenticated)
                return this.Unauthorized();
            
            var games = await this.gameService.GetAsync(
                new GetOptions<Game, string, GameViewModel>
                {
                    Projection = new ProjectionOptions<Game, GameViewModel>(
                        g => GameFactory.CreateGameViewModel(g)),
                });

            return this.Ok(games);
        }
        
        [HttpPost]
        public async Task<ActionResult<CreateGameResponseModel>> Create(GameViewModel gameViewModel, CancellationToken cancellationToken)
        {
            if (!this.AuthenticationService.Authenticated)
                return this.Unauthorized();

            if (gameViewModel == null)
                return this.BadRequest(nameof(gameViewModel));

            var operationResult = await this.gameService.CreateAsync(gameViewModel.ToGame(), cancellationToken);
            var responseModelOperationResult = operationResult.ChangeObjectType(GameFactory.CreateGameResponseModel(operationResult.Object));

            return this.FromOperationResult(responseModelOperationResult);
        }
        
        [HttpPut]
        public async Task<ActionResult<UpdateGameResponseModel>> Update(UpdateGameModel updateGameModel, CancellationToken cancellationToken)
        {
            if (!this.AuthenticationService.Authenticated)
                return this.Unauthorized();

            if (updateGameModel == null)
                return this.BadRequest(nameof(updateGameModel));

            var operationResult = await this.gameService.UpdateAsync(new object[] { updateGameModel.Id }, updateGameModel.ToGame(), cancellationToken);
            var responseModelOperationResult = operationResult.ChangeObjectType(GameFactory.UpdateGameResponseModel(operationResult.Object));

            return this.FromOperationResult(responseModelOperationResult);
        }
        
        [HttpDelete]
        public async Task<ActionResult<DeleteGameResponseModel>> Delete(int id, CancellationToken cancellationToken)
        {
            if (!this.AuthenticationService.Authenticated)
                return this.Unauthorized();

            var operationResult = await this.gameService.SoftDeleteAsync(new object[] { id }, cancellationToken);
            var responseModelOperationResult = operationResult.ChangeObjectType(GameFactory.DeleteGameResponseModel());

            return this.FromOperationResult(responseModelOperationResult);
        }
    }
}