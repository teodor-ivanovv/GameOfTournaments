namespace GameOfTournaments.Web.Models.Game
{
    using System;

    /// <summary>
    /// Create game response model.
    /// </summary>
    public class CreateGameResponseModel : GameViewModel
    {
        public DateTimeOffset CreatedDate { get; set; }
    }
}