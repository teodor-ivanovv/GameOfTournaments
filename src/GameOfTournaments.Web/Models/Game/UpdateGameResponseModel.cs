namespace GameOfTournaments.Web.Models.Game
{
    using System;

    /// <summary>
    /// Update game response model.
    /// </summary>
    public class UpdateGameResponseModel : GameViewModel
    {
        public DateTimeOffset? UpdatedDate { get; set; }
    }
}