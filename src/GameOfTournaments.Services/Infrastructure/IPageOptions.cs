namespace GameOfTournaments.Services.Infrastructure
{
    /// <summary>
    /// Represents an interface, specifying pagination options.
    /// </summary>
    public interface IPageOptions
    {
        /// <summary>
        /// Gets the page number.
        /// </summary>
        int Page { get; }

        /// <summary>
        /// Gets the count of entities per page.
        /// </summary>
        int Count { get; }
    }
}