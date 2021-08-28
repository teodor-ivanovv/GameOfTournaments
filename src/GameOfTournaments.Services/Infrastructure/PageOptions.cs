namespace GameOfTournaments.Services.Infrastructure
{
    using System;

    public class PageOptions : IPageOptions
    {
        public PageOptions(int page, int count)
        {
            if (page <= 0)
                throw new ArgumentOutOfRangeException(nameof(page), "Argument page must be greater than 0.");

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Argument count must be greater than 0.");

            this.Page = page;
            this.Count = count;
        }

        /// <inheritdoc />
        public int Page { get; }

        /// <inheritdoc />
        public int Count { get; }
    }
}