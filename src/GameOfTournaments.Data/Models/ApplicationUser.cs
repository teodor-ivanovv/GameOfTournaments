namespace GameOfTournaments.Data.Models
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser<int>
    {
        public int Age { get; set; }
        
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public bool Male { get; set; }
    }
}