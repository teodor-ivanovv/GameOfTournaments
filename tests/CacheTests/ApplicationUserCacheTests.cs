namespace CacheTests
{
    using System.Linq;
    using System.Threading.Tasks;
    using GameOfTournaments.Web.Cache.ApplicationUsers;
    using GameOfTournamentsTests;
    using Xunit;

    public class ApplicationUserCacheTests : GameOfTournamentsBaseTests
    {
        private readonly IApplicationUserCache _applicationUserCache;

        public ApplicationUserCacheTests()
        {
            this._applicationUserCache = new ApplicationUserCache();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ShouldReturnNullIfNoCacheEntryFound(int id)
        {
            // Arrange
            // Act
            var cached = this._applicationUserCache.Get(id);

            // Assert
            Assert.Null(cached);
        }

        [Fact]
        public void ShouldReturnValidCacheEntry()
        {
            // Arrange
            for (var i = 0; i < 100; i++)
            {
                var userCacheModel = new ApplicationUserCacheModel
                {
                    Id = i + 1,
                    Roles = Enumerable.Range(0, i + 1).Select(_ => i.ToString()).ToList(),
                };

                this._applicationUserCache.Cache(userCacheModel);
            }

            // Act
            // Assert
            Assert.Equal(100, this._applicationUserCache.Count);

            for (var i = 0; i < 100; i++)
            {
                var cached = this._applicationUserCache.Get(i + 1);
                Assert.NotNull(cached);

                var roles = Enumerable.Range(0, i + 1).Select(_ => i.ToString()).ToList();
                Assert.True(DeepEqual(cached.Roles, roles));
            }
        }

        [Fact]
        public void ShouldBeThreadSafe()
        {
            var incrementId = 0;
            var lockObject = new object();
            
            // Arrange
            Parallel.For(
                0,
                10,
                _ =>
                {
                    for (var i = 0; i < 100; i++)
                    {
                        int id;
                        lock (lockObject)
                            id = ++incrementId;
                        
                        var userCacheModel = new ApplicationUserCacheModel
                        {
                            Id = id, 
                            Roles = Enumerable.Range(0, id + 1).Select(_ => id.ToString()).ToList(),
                        };

                        this._applicationUserCache.Cache(userCacheModel);
                    }
                });

            // Act
            // Assert
            Assert.Equal(1000, this._applicationUserCache.Count);

            incrementId = 0;
            Parallel.For(
                0,
                10,
                _ =>
                {
                    for (var i = 0; i < 100; i++)
                    {
                        int id;
                        lock (lockObject)
                            id = ++incrementId;
                        
                        var cached = this._applicationUserCache.Get(id);
                        Assert.NotNull(cached);

                        var roles = Enumerable.Range(0, id + 1).Select(_ => id.ToString()).ToList();
                        Assert.True(DeepEqual(cached.Roles, roles));
                    }
                });
        }
    }
}