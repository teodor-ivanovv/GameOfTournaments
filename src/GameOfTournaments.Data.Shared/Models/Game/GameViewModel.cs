namespace GameOfTournaments.Data.Factories.Models.Game
{
    using System.ComponentModel.DataAnnotations;
    using GameOfTournaments.Data.Models;
    using static GameOfTournaments.Shared.GameConstants;

    /// <summary>
    /// Create game view model and get game view model.
    /// </summary>
    public class GameViewModel
    {
        [Required]
        [MinLength(NameMinimumLength)]
        [MaxLength(NameMaximumLength)]
        public string Name { get; set; }

        [Required]
        [MinLength(DescriptionMinimumLength)]
        [MaxLength(DescriptionMaximumLength)]
        public string Description { get; set; }

        public virtual Game ToGame() => new() { Name = this.Name, Description = this.Description };
    }
}