namespace GameOfTournaments.Data.Factories.Models.Game
{
    using System;

    /// <summary>
    /// Update game response model.
    /// </summary>
    public class UpdateGameViewModel : GameViewModel
    {
        public DateTimeOffset? UpdatedDate { get; set; }
    }
}