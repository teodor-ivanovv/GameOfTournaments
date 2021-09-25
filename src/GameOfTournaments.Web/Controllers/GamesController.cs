namespace GameOfTournaments.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services;
    using GameOfTournaments.Services.Infrastructure;
    using GameOfTournaments.Web.Cache.ApplicationUsers;
    using GameOfTournaments.Web.Factories;
    using GameOfTournaments.Web.Models;
    using GameOfTournaments.Web.Models.Game;
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
        public async Task<ActionResult<IEnumerable<GameViewModel>>> GetGames()
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
        public async Task<ActionResult<CreateGameResponseModel>> CreateGame(GameViewModel gameViewModel)
        {
            if (!this.AuthenticationService.Authenticated)
                return this.Unauthorized();

            if (gameViewModel == null)
                return this.BadRequest(nameof(gameViewModel));

            var operationResult = await this.gameService.CreateAsync(gameViewModel.ToGame());
            var responseModelOperationResult = operationResult.ChangeObjectType(GameFactory.CreateCreateGameResponseModel(operationResult.Object));

            return this.FromOperationResult(responseModelOperationResult);
        }
    }
}