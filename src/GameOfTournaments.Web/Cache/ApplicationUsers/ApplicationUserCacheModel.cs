namespace GameOfTournaments.Web.Cache.ApplicationUsers
{
    using System.Collections.Generic;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Shared;

    public class ApplicationUserCacheModel
    {
        public int Id { get; set; }

        public List<PermissionModel> Permissions { get; set; } = new();
        
        // IP checks, browser (client) checks
    }
}