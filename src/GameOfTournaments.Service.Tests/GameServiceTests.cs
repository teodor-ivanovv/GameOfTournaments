namespace GameOfTournaments.Service.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services;
    using GameOfTournaments.Services.Infrastructure;
    using GameOfTournaments.Shared;
    using GameOfTournamentsTests;
    using Xunit;

    public class GameServiceTests : BaseTests
    {
        private readonly GetOptions<Game, int> gameGetOptions = new()
        {
            Sort = new SortOptions<Game, int>(true, g => g.Id),
            Pagination = new PageOptions(1, 1000),
        };
        
        [Fact]
        public async Task GetShouldReturnNothing()
        {
            // Arrange
            // Act
            var games = await this.GameService.GetAsync(this.gameGetOptions);

            // Assert
            Assert.NotNull(games);
            Assert.Empty(games);
        }
        
        [Fact]
        public async Task GetShouldReturnNothingIfUserIsAuthenticated()
        {
            // Arrange
            this.AuthenticateUser();
            
            // Act
            var games = await this.GameService.GetAsync(this.gameGetOptions);

            // Assert
            Assert.NotNull(games);
            Assert.Empty(games);
        }
        
        [Fact]
        public async Task GetShouldReturnNothingIfUserIsNotAuthenticated()
        {
            // Arrange
            this.AuthenticateUser(Permissions.CanCreateGame);
            var games = this.CreateGameModels(100);

            var operationResult = await this.GameService.CreateManyAsync(games);
            Assert.NotNull(operationResult);
            Assert.True(operationResult.Success);
            Assert.Empty(operationResult.Errors);
            Assert.NotNull(operationResult.Object);
            Assert.NotEmpty(operationResult.Object);
            Assert.Equal(100, operationResult.Object.Count());
            
            // Act
            this.AuthenticateUser((IAuthenticationContext)null);

            var databaseGames = await this.GameService.GetAsync(this.gameGetOptions);
            
            // Assert
            Assert.NotNull(databaseGames);
            Assert.Empty(databaseGames);
        }
        
        [Fact]
        public async Task CreateGameShouldFailIfUserIsNotAuthenticated()
        {
            // Arrange
            var game = this.CreateGameModel();

            // Act
            var operationResult = await this.GameService.CreateAsync(game);

            // Assert
            Assert.NotNull(operationResult);
            Assert.False(operationResult.Success);
            Assert.True(operationResult.Errors.Any());
            
            // Authenticate user and assert no game created for sure
            this.AuthenticateUser();
            var games = await this.GameService.GetAsync(this.gameGetOptions);
            Assert.NotNull(games);
            Assert.Empty(games);

            await this.AssertAuditLogAsync(Permissions.CanCreateGame, null, false);
        }

        [Fact]
        public async Task CreateGameShouldFailIfUserIsNotInRole()
        {
            // Arrange
            this.AuthenticateUser();
            var game = this.CreateGameModel();

            // Act
            var operationResult = await this.GameService.CreateAsync(game);

            // Assert
            Assert.NotNull(operationResult);
            Assert.False(operationResult.Success);
            Assert.True(operationResult.Errors.Any());
        }
        
        [Fact]
        public async Task CreateGameShouldSucceed()
        {
            // Arrange
            
            // Seed games
            // Act
            // Assert
        }

        private Game CreateGameModel()
        {
            var game = new Game
            {
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
            };

            return game;
        }
        
        private IEnumerable<Game> CreateGameModels(int count)
        {
            for (var i = 0; i < count; i++)
                yield return this.CreateGameModel();
        }
    }
}