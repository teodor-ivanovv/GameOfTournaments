namespace GameOfTournaments.Data.Factories.Models.Game
{
    using System.ComponentModel.DataAnnotations;
    using GameOfTournaments.Data.Models;

    /// <summary>
    /// Update game input model.
    /// </summary>
    public class UpdateGameInputModel : GameViewModel
    {
        [Range(0, int.MaxValue)]
        public int Id { get; set; }

        public override Game ToGame()
        {
            var game = base.ToGame();
            game.Id = this.Id;

            return game;
        }
    }
}