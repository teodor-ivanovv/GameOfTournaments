namespace GameOfTournaments.Data.Models
{
    using GameOfTournaments.Data.Infrastructure;

    public class ApplicationUserAccount : IIdentifiable<int>
    {
        public int Id { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public int ApplicationUserId { get; set; }

        public int CreateTournamentsPerDay { get; set; }
    }
}