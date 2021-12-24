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
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class GamesController : Controller
    {
        [NotNull]
        private readonly IGameService _gameService;

        public GamesController(
            [NotNull] IHttpContextAccessor httpContextAccessor,
            [NotNull] IAuthenticationService authenticationService,
            [NotNull] IGameService gameService,
            [NotNull] IApplicationUserCache applicationUserCache)
            : base(httpContextAccessor, authenticationService, applicationUserCache)
        {
            this._gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameViewModel>>> Get()
        {
            if (!this.AuthenticationService.Authenticated)
                return this.Unauthorized();

            var games = await this._gameService.GetAsync(
                new GetOptions<Game, string, GameViewModel>
                {
                    Projection = new ProjectionOptions<Game, GameViewModel>(
                        g => GameFactory.CreateGameViewModel(g)),
                });

            return this.Ok(games);
        }

        [HttpPost]
        public async Task<ActionResult<CreateGameViewModel>> Create(
            [NotNull] GameViewModel gameViewModel,
            CancellationToken cancellationToken)
        {
            if (!this.AuthenticationService.Authenticated)
                return this.Unauthorized();

            var operationResult = await this._gameService.CreateAsync(gameViewModel.ToGame(), cancellationToken);
            var responseModelOperationResult = operationResult.ChangeObjectType(GameFactory.CreateGameResponseModel(operationResult.Object));

            return this.FromOperationResult(responseModelOperationResult);
        }

        [HttpPut]
        public async Task<ActionResult<UpdateGameViewModel>> Update(
            [NotNull] UpdateGameInputModel updateGameInputModel,
            CancellationToken cancellationToken)
        {
            if (!this.AuthenticationService.Authenticated)
                return this.Unauthorized();

            var operationResult = await this._gameService.UpdateAsync(
                new object[]
                {
                    updateGameInputModel.Id,
                },
                updateGameInputModel.ToGame(),
                cancellationToken);
            var responseModelOperationResult = operationResult.ChangeObjectType(GameFactory.UpdateGameResponseModel(operationResult.Object));

            return this.FromOperationResult(responseModelOperationResult);
        }

        [HttpDelete]
        public async Task<ActionResult<DeleteGameViewModel>> Delete(int id, CancellationToken cancellationToken)
        {
            if (!this.AuthenticationService.Authenticated)
                return this.Unauthorized();

            var operationResult = await this._gameService.SoftDeleteAsync(
                new object[] { id },
                cancellationToken);
            var responseModelOperationResult = operationResult.ChangeObjectType(GameFactory.DeleteGameResponseModel());

            return this.FromOperationResult(responseModelOperationResult);
        }
    }
}