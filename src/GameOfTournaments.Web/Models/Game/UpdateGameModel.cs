namespace GameOfTournaments.Web.Models.Game
{
    using System.ComponentModel.DataAnnotations;
    using GameOfTournaments.Data.Models;

    public class UpdateGameModel : GameViewModel
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