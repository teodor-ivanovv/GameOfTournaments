namespace GameOfTournaments.Web.Cache.ApplicationUsers
{
    using System.Collections.Generic;

    public class ApplicationUserCacheModel
    {
        public int Id { get; set; }

        public List<string> Roles { get; set; } = new();
        
        // IP checks, browser (client) checks
    }
}