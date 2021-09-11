namespace GameOfTournaments.Web.Cache.ApplicationUsers
{
    using System;
    using Ardalis.GuardClauses;
    using Microsoft.Extensions.Caching.Memory;

    public class ApplicationUserCache : IApplicationUserCache, IDisposable
    {
        private readonly MemoryCache memoryCache;

        // Set cache options.
        private static readonly MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
            // Keep in cache for this time, reset time if accessed.
            .SetSlidingExpiration(TimeSpan.FromDays(7));

        public ApplicationUserCache()
        {
            this.memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        public ApplicationUserCacheModel Get(int id)
            => this.memoryCache.Get<ApplicationUserCacheModel>(id);

        public void Cache(ApplicationUserCacheModel applicationUserCacheModel)
        {
            Guard.Against.Null(applicationUserCacheModel, nameof(applicationUserCacheModel));
            this.memoryCache.Set(applicationUserCacheModel.Id, applicationUserCacheModel, cacheEntryOptions);
        }

        public void Dispose()
        {
            this.memoryCache.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}