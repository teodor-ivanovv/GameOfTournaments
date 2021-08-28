﻿namespace GameOfTournaments.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class LoginUserModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}