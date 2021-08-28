namespace GameOfTournaments.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterUserModel
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string MiddleName { get; set; }
        
        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Range(18, 150)]
        public int Age { get; set; }

        public bool Male { get; set; }

        [Required]
        public string Password { get; set; }
        
        [Phone]
        public string PhoneNumber { get; set; }
    }
}