namespace GameOfTournaments.Service.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services.Infrastructure;
    using GameOfTournaments.Shared;
    using GameOfTournamentsTests;
    using Xunit;

    public class GameServiceTests : BaseTests
    {
        private readonly GetOptions<Game, int> getOptions = new()
        {
            Sort = new SortOptions<Game, int>(true, g => g.Id),
            Pagination = new PageOptions(1, 1000),
        };
        
        [Fact]
        public async Task GetShouldReturnNothing()
        {
            // Arrange
            // Act
            var operationResult = await this.GameService.GetAsync(this.getOptions);

            // Assert
            this.AssertOperationResult(operationResult, false);
        }
        
        [Fact]
        public async Task GetShouldReturnNothingIfUserIsAuthenticated()
        {
            // Arrange
            this.AuthenticateUser();
            
            // Act
            var operationResult = await this.GameService.GetAsync(this.getOptions);

            // Assert
            this.AssertOperationResult(operationResult, 0);
        }
        
        [Fact]
        public async Task GetShouldReturnNothingIfUserIsNotAuthenticated()
        {
            // Arrange
            this.AuthenticateUser(new PermissionModel { Scope = PermissionScope.Game, Permissions = Permissions.Create });
            var games = this.CreateGameModels(100);

            var operationResult = await this.GameService.CreateManyAsync(games);
            this.AssertOperationResult(operationResult, 100);
            
            // Act
            this.AuthenticateUser((IAuthenticationContext)null);

            var databaseGamesOperationResult = await this.GameService.GetAsync(this.getOptions);
            
            // Assert
            this.AssertOperationResult(databaseGamesOperationResult, false);
        }
        
        [Fact]
        public async Task GetShouldReturnNothingIfUserIsNotAuthenticatedCreatingGamesOneByOne()
        {
            // Arrange
            this.AuthenticateUser(new PermissionModel { Scope = PermissionScope.Game, Permissions = Permissions.Create });
            for (var i = 0; i < 100; i++)
            {
                var game = this.CreateGameModel();
                var operationResult = await this.GameService.CreateAsync(game);
             
                this.AssertOperationResult(operationResult);
            }

            var getGamesOperationResult = await this.GameService.GetAsync(this.getOptions);
            this.AssertOperationResult(getGamesOperationResult, 100);
            
            // Act
            this.AuthenticateUser((IAuthenticationContext)null);

            var databaseGamesOperationResult = await this.GameService.GetAsync(this.getOptions);
            
            // Assert
            this.AssertOperationResult(databaseGamesOperationResult, false);
        }
        
        [Fact]
        public async Task CreateGameShouldFailIfUserIsNotAuthenticated()
        {
            // Arrange
            var game = this.CreateGameModel();

            // Act
            var operationResult = await this.GameService.CreateAsync(game);

            // Assert
            this.AssertOperationResult(operationResult, false);
            
            // Authenticate user and assert no game created for sure
            this.AuthenticateUser();
            var gamesOperationResult = await this.GameService.GetAsync(this.getOptions);
            this.AssertOperationResult(gamesOperationResult, 0);

            // await this.AssertAuditLogAsync(Permissions.CanCreateGame, null, false);
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
            this.AssertOperationResult(operationResult, false);
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