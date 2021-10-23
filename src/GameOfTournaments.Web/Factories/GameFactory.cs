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
        
        public static DeleteGameResponseModel DeleteGameResponseModel()
        {
            return default;
        }
    }
}