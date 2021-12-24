namespace GameOfTournaments.Data.Factories.Models.Game
{
    using System;

    /// <summary>
    /// Create game view/response model.
    /// </summary>
    public class CreateGameViewModel : GameViewModel
    {
        public DateTimeOffset CreatedDate { get; set; }
    }
}