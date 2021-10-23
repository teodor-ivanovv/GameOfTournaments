namespace GameOfTournaments.Shared
{
    /// <summary>
    /// An enumeration representing the scope of the assigned permissions.
    /// </summary>
    public enum PermissionScope
    {
        /// <summary>
        /// Game scope related to games.
        /// </summary>
        Game = 0,
        
        /// <summary>
        /// Tournament scope related to tournaments.
        /// </summary>
        Tournament = 1,
    }
}