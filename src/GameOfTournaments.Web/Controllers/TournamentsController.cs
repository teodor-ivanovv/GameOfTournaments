namespace GameOfTournaments.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Factories.Models.Tournament;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services;
    using GameOfTournaments.Services.Infrastructure;
    using GameOfTournaments.Web.Cache.ApplicationUsers;
    using GameOfTournaments.Web.Factories;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class TournamentsController : Controller
    {
        [NotNull]
        private readonly ITournamentService _tournamentService;

        public TournamentsController(
            [NotNull] IHttpContextAccessor httpContextAccessor,
            [NotNull] IAuthenticationService authenticationService,
            [NotNull] IApplicationUserCache applicationUserCache,
            [NotNull] ITournamentService tournamentService)
            : base(httpContextAccessor, authenticationService, applicationUserCache)
        {
            this._tournamentService = tournamentService ?? throw new ArgumentNullException(nameof(tournamentService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TournamentViewModel>>> Get([FromQuery] int page = 1, [FromQuery] int count = 100)
        {
            if (!this.AuthenticationService.Authenticated)
                return this.Unauthorized();

            var tournaments = await this._tournamentService.GetAsync(
                new GetOptions<Tournament, int, TournamentViewModel>
                {
                    Projection = new ProjectionOptions<Tournament, TournamentViewModel>(
                        g => TournamentFactory.CreateTournamentViewModel(g)),
                    Pagination = new PageOptions(page, count),
                    Sort = new SortOptions<Tournament, int>(true, t => t.Id),
                });

            return this.Ok(tournaments);
        }

        [HttpPost]
        public async Task<ActionResult<CreateTournamentViewModel>> Create(
            [NotNull] CreateTournamentViewModel tournamentViewModel,
            CancellationToken cancellationToken)
        {
            if (!this.AuthenticationService.Authenticated)
                return this.Unauthorized();

            var operationResult = await this._tournamentService.CreateAsync(tournamentViewModel.ToTournament(), cancellationToken);
            var responseModelOperationResult = operationResult.ChangeObjectType(TournamentFactory.CreateTournamentResponseModel(operationResult.Object));

            return this.FromOperationResult(responseModelOperationResult);
        }
    }
}