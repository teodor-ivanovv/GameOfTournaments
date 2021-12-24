namespace GameOfTournaments.Web.Factories
{
    using GameOfTournaments.Data.Factories.Models.Game;
    using GameOfTournaments.Data.Models;

    public static class GameFactory
    {
        public static GameViewModel CreateGameViewModel(Game game)
        {
            if (game == default)
                return default;
            
            return new GameViewModel { Name = game.Name, Description = game.Description };
        }

        public static CreateGameViewModel CreateGameResponseModel(Game game)
        {
            if (game == default)
                return default;
            
            return new CreateGameViewModel { Name = game.Name, Description = game.Description, CreatedDate = game.Created };
        }

        public static UpdateGameViewModel UpdateGameResponseModel(Game game)
        {
            if (game == default)
                return default;
            
            return new UpdateGameViewModel { Name = game.Name, Description = game.Description, UpdatedDate = game.LastModified };
        }
        
        public static DeleteGameViewModel DeleteGameResponseModel()
        {
            return default;
        }
    }
}