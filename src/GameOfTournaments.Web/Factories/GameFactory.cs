namespace GameOfTournaments.Web.Factories
{
    using Ardalis.GuardClauses;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Web.Models;

    public static class GameFactory
    {
        public static GameViewModel Create(Game game)
        {
            Guard.Against.Null(game, nameof(game));
            return new GameViewModel { Name = game.Name, Description = game.Description };
        }
    }
}