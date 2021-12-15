namespace GameOfTournaments.Web.Cache.ApplicationUsers
{
    using System;
    using Ardalis.GuardClauses;
    using Microsoft.Extensions.Caching.Memory;

    public class ApplicationUserCache : IApplicationUserCache, IDisposable
    {
        // Set cache options.
        private static readonly MemoryCacheEntryOptions _cacheEntryOptions = new MemoryCacheEntryOptions()
            // Keep in cache for this time, reset time if accessed.
            .SetSlidingExpiration(TimeSpan.FromDays(7));

        private readonly MemoryCache _memoryCache;

        public ApplicationUserCache()
        {
            this._memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        public long Count => this._memoryCache.Count;

        public ApplicationUserCacheModel Get(int id) => this._memoryCache.Get<ApplicationUserCacheModel>(id);

        public void Cache(ApplicationUserCacheModel applicationUserCacheModel)
        {
            Guard.Against.Null(applicationUserCacheModel, nameof(applicationUserCacheModel));
            this._memoryCache.Set(applicationUserCacheModel.Id, applicationUserCacheModel, _cacheEntryOptions);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this._memoryCache.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}