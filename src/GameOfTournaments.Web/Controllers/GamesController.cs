namespace GameOfTournaments.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services;
    using GameOfTournaments.Services.Infrastructure;
    using GameOfTournaments.Web.Factories;
    using GameOfTournaments.Web.Models;
    using Microsoft.AspNetCore.Mvc;

    public class GamesController : Controller
    {
        private readonly IGameService gameService;

        public GamesController(IGameService gameService)
        {
            this.gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameViewModel>>> Games()
        {
            // TODO: Validate user in base controller

            var games = await this.gameService.GetAsync(
                new GetOptions<Game, string, GameViewModel>
                {
                    Projection = new ProjectionOptions<Game, GameViewModel>(
                        g => GameFactory.Create(g))
                });

            return this.Ok(games);
        }
    }
}