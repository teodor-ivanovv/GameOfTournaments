namespace GameOfTournaments.Shared
{
    using System;

    [Flags]
    public enum Permissions
    {
        View = 1 << 0,
        Create = 1 << 1,
        Update = 1 << 2,
        SoftDelete = 1 << 3,
        HardDelete = 1 << 4,
    }
}