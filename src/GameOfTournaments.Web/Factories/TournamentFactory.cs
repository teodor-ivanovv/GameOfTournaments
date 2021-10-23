namespace GameOfTournaments.Web.Factories
{
    using GameOfTournaments.Data.Factories.Models.Tournament;
    using GameOfTournaments.Data.Models;
    using JetBrains.Annotations;

    /// <summary>
    /// Tournament factory for creating tournament related objects and converting them from one to another.
    /// </summary>
    public class TournamentFactory
    {
        /// <summary>
        /// Creates a <see cref="TournamentViewModel"/> based on the given <paramref name="tournament"/>.
        /// </summary>
        /// <param name="tournament">The <see cref="Tournament"/> to use when creating the <see cref="TournamentViewModel"/> result.</param>
        /// <returns>The created instance of <see cref="TournamentViewModel"/> or a default value when the given <param name="tournament"></param> is null.</returns>
        public static TournamentViewModel CreateTournamentViewModel([NotNull] Tournament tournament)
        {
            if (tournament == null)
                return default;

            return new TournamentViewModel
            {
                Id = tournament.Id,
                Name = tournament.Name,
                Description = tournament.Description,
                Created = tournament.Created,
                Start = tournament.Start,
                End = tournament.End,
                Status = tournament.Status,
                JoinFee = tournament.JoinFee,
                MaximumCompetitors = tournament.MaximumCompetitors,
                FirstPlacePrize = tournament.FirstPlacePrize,
                SecondPlacePrize = tournament.SecondPlacePrize,
                ThirdPlacePrize = tournament.ThirdPlacePrize,
                MinimumCompetitors = tournament.MinimumCompetitors,
            };
        }

        /// <summary>
        /// Creates a <see cref="CreateTournamentViewModel"/> based on the given <paramref name="tournament"/>.
        /// </summary>
        /// <param name="tournament">The <see cref="Tournament"/> to use when creating the <see cref="CreateTournamentViewModel"/> result.</param>
        /// <returns>The created instance of <see cref="CreateTournamentViewModel"/> or a default value when the given <param name="tournament"></param> is null.</returns>
        public static CreateTournamentViewModel CreateTournamentResponseModel([NotNull] Tournament tournament)
        {
            if (tournament == null)
                return default;

            return new TournamentViewModel
            {
                Id = tournament.Id,
                Created = tournament.Created,
                Status = tournament.Status,
                Name = tournament.Name,
                Description = tournament.Description,
                JoinFee = tournament.JoinFee,
                Public = tournament.Public,
                FirstPlacePrize = tournament.FirstPlacePrize,
                SecondPlacePrize = tournament.SecondPlacePrize,
                ThirdPlacePrize = tournament.ThirdPlacePrize,
                MaximumCompetitors = tournament.MaximumCompetitors,
                MinimumCompetitors = tournament.MinimumCompetitors,
                Start = tournament.Start,
                End = tournament.End,
            };
        }
    }
}