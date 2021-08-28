namespace GameOfTournaments.Data
{
    using GameOfTournaments.Data.Models;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
    }
}