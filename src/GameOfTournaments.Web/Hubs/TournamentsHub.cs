namespace GameOfTournaments.Web.Hubs
{
    using System;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Factories.Models.Tournament;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services;
    using GameOfTournaments.Services.Infrastructure;
    using GameOfTournaments.Web.Factories;
    using Microsoft.AspNetCore.SignalR;

    public class TournamentsHub : Hub
    {
        private readonly ITournamentService _tournamentService;

        public TournamentsHub(ITournamentService tournamentService)
        {
            this._tournamentService = tournamentService ?? throw new ArgumentNullException(nameof(tournamentService));
        }

        public override async Task OnConnectedAsync()
        {
            var operationResult = await this._tournamentService.GetAsync(
                new GetOptions<Tournament, int, TournamentViewModel>
                {
                    Projection = new ProjectionOptions<Tournament, TournamentViewModel>(
                        g => TournamentFactory.CreateTournamentViewModel(g)),
                    Sort = new SortOptions<Tournament, int>(true, t => t.Id),
                });
            
            await this.Clients.Caller.SendAsync("Snapshot", operationResult);
        }
    }
}