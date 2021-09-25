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
            if (game == default)
                return default;
            
            return new GameViewModel { Name = game.Name, Description = game.Description };
        }

        public static CreateGameResponseModel CreateGameResponseModel(Game game)
        {
            if (game == default)
                return default;
            
            return new CreateGameResponseModel { Name = game.Name, Description = game.Description, CreatedDate = game.Created };
        }

        public static UpdateGameResponseModel UpdateGameResponseModel(Game game)
        {
            if (game == default)
                return default;
            
            return new UpdateGameResponseModel { Name = game.Name, Description = game.Description, UpdatedDate = game.LastModified };
        }
    }
}