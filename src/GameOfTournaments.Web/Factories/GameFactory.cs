namespace GameOfTournaments.Web.Factories
{
    using Ardalis.GuardClauses;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Web.Models;
    using GameOfTournaments.Web.Models.Game;

    public static class GameFactory
    {
        public static GameViewModel CreateGameViewModel(Game game)
        {
            Guard.Against.Null(game, nameof(game));
            return new GameViewModel { Name = game.Name, Description = game.Description };
        }
        
        public static CreateGameResponseModel CreateCreateGameResponseModel(Game game)
        {
            Guard.Against.Null(game, nameof(game));
            return new CreateGameResponseModel { Name = game.Name, Description = game.Description, CreatedDate = game.Created };
        }
    }
}