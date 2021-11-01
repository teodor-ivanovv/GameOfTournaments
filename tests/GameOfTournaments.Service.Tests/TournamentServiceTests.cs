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

    public class TournamentServiceTests : BaseTests
    {
        private readonly GetOptions<Tournament, int> getOptions = new()
        {
            Sort = new SortOptions<Tournament, int>(true, t => t.Id),
            Pagination = new PageOptions(1, 1000),
        };
        
        [Fact]
        public async Task GetShouldReturnNothing()
        {
            // Arrange
            // Act
            var operationResult = await this.TournamentService.GetAsync(this.getOptions);

            // Assert
            this.AssertOperationResult(operationResult, false);
        }
        
        [Fact]
        public async Task GetShouldReturnNothingIfUserIsAuthenticated()
        {
            // Arrange
            this.AuthenticateUser();
            
            // Act
            var operationResult = await this.TournamentService.GetAsync(this.getOptions);

            // Assert
            this.AssertOperationResult(operationResult, 0);
        }
        
        [Fact]
        public async Task GetShouldReturnNothingIfUserIsNotAuthenticated()
        {
            // Arrange
            this.AuthenticateUser(new PermissionModel { Scope = PermissionScope.Tournament, Permissions = Permissions.Create });
            var tournaments = this.CreateTournamentModels(10);

            var operationResult = await this.TournamentService.CreateManyAsync(tournaments);
            this.AssertOperationResult(operationResult, 10);
            
            // Act
            this.AuthenticateUser((IAuthenticationContext)null);

            var databaseTournamentsOperationResult = await this.TournamentService.GetAsync(this.getOptions);
            
            // Assert
            this.AssertOperationResult(databaseTournamentsOperationResult, false);
        }

        [Fact]
        public async Task GetShouldReturnNothingIfUserIsNotAuthenticatedAndTournamentsCreationFailed()
        {
            // Arrange
            this.AuthenticateUser(new PermissionModel { Scope = PermissionScope.Tournament, Permissions = Permissions.Create });
            var tournaments = this.CreateTournamentModels(11);

            var operationResult = await this.TournamentService.CreateManyAsync(tournaments);
            this.AssertOperationResult(operationResult, 0, success: false);
            
            // Act
            this.AuthenticateUser((IAuthenticationContext)null);

            var databaseTournamentsOperationResult = await this.TournamentService.GetAsync(this.getOptions);
            
            // Assert
            this.AssertOperationResult(databaseTournamentsOperationResult, false);
        }
        
        [Fact]
        public async Task GetShouldReturnNothingIfUserIsNotAuthenticatedAndFailIfMaximumCreationTournamentPerDayReached()
        {
            // Arrange
            this.AuthenticateUser(new PermissionModel { Scope = PermissionScope.Tournament, Permissions = Permissions.Create });
            var tournaments = this.CreateTournamentModels(100);

            var operationResult = await this.TournamentService.CreateManyAsync(tournaments);
            this.AssertOperationResult(operationResult, success: false);
            
            var databaseTournamentsOperationResult = await this.TournamentService.GetAsync(this.getOptions);
            
            // Assert
            this.AssertOperationResult(databaseTournamentsOperationResult, 0);
            
            // Act
            this.AuthenticateUser((IAuthenticationContext)null);
            databaseTournamentsOperationResult = await this.TournamentService.GetAsync(this.getOptions);
            
            // Assert
            this.AssertOperationResult(databaseTournamentsOperationResult, false);
        }
        
        [Fact]
        public async Task GetShouldReturnNothingIfUserIsNotAuthenticatedCreatingTournamentsOneByOne()
        {
            // Arrange
            this.AuthenticateUser(new PermissionModel { Scope = PermissionScope.Tournament, Permissions = Permissions.Create });
            for (var i = 0; i < 10; i++)
            {
                var tournament = this.CreateTournamentModel();
                var operationResult = await this.TournamentService.CreateAsync(tournament);
                
                this.AssertOperationResult(operationResult);
            }

            var getTournamentsOperationResult = await this.TournamentService.GetAsync(this.getOptions);
            this.AssertOperationResult(getTournamentsOperationResult, 10);
            
            // Act
            this.AuthenticateUser((IAuthenticationContext)null);

            var databaseTournamentsOperationResult = await this.TournamentService.GetAsync(this.getOptions);
            
            // Assert
            this.AssertOperationResult(databaseTournamentsOperationResult, false);
        }
        
        [Fact]
        public async Task GetShouldReturnNothingIfUserIsNotAuthenticatedCreatingTournamentsOneByOneAndFailIfMaximumCreationTournamentPerDayReached()
        {
            // Arrange
            this.AuthenticateUser(new PermissionModel { Scope = PermissionScope.Tournament, Permissions = Permissions.Create });
            for (var i = 0; i < 100; i++)
            {
                var tournament = this.CreateTournamentModel();
                var operationResult = await this.TournamentService.CreateAsync(tournament);

                Assert.NotNull(operationResult);

                if (i < 10)
                {
                    this.AssertOperationResult(operationResult);
                }
                else
                {
                    this.AssertOperationResult(operationResult, false);
                }
            }

            var getTournamentsOperationResult = await this.TournamentService.GetAsync(this.getOptions);
            this.AssertOperationResult(getTournamentsOperationResult, 10);
            
            // Act
            this.AuthenticateUser((IAuthenticationContext)null);
            var databaseTournamentsOperationResult = await this.TournamentService.GetAsync(this.getOptions);
            
            // Assert
            this.AssertOperationResult(databaseTournamentsOperationResult, false);
        }

        [Fact]
        public async Task UserShouldNotBeAbleToSeePrivateTournaments()
        {
            // Arrange
            this.AuthenticateUser(new PermissionModel { Scope = PermissionScope.Tournament, Permissions = Permissions.Create });
            
            var tournaments = this.CreateTournamentModels(5, privateTournaments: true);
            var operationResult = await this.TournamentService.CreateManyAsync(tournaments);
            
            // Assert
            this.AssertOperationResult(operationResult, 5);

            // Act
            var getPrivateTournamentsOperationResult = await this.TournamentService.GetAsync(this.getOptions);
            this.AssertOperationResult(getPrivateTournamentsOperationResult, 5);

            // Switch user
            this.AuthenticateUser(new PermissionModel { Scope = PermissionScope.Tournament, Permissions = Permissions.Create });
            
            getPrivateTournamentsOperationResult = await this.TournamentService.GetAsync(this.getOptions);
            
            // Assert
            this.AssertOperationResult(getPrivateTournamentsOperationResult, 0);
        }
        
        [Fact]
        public async Task CreatingAndRetrievingShouldWorkAppropriatelyForUsers()
        {
            // Arrange
            var firstUserId = this.AuthenticateUser(new PermissionModel { Scope = PermissionScope.Tournament, Permissions = Permissions.Create });
            
            var privateTournaments = this.CreateTournamentModels(5, privateTournaments: true);
            var operationResult = await this.TournamentService.CreateManyAsync(privateTournaments);
            
            // Assert
            this.AssertOperationResult(operationResult, 5);
            
            // Act
            var publicTournaments = this.CreateTournamentModels(2);
            operationResult = await this.TournamentService.CreateManyAsync(publicTournaments);
            
            // Assert
            this.AssertOperationResult(operationResult, 2);
            
            // Act
            var getTournamentsOperationResult = await this.TournamentService.GetAsync(this.getOptions);
            
            // Assert
            this.AssertOperationResult(getTournamentsOperationResult, 7);

            // Switch user
            this.AuthenticateUser(new PermissionModel { Scope = PermissionScope.Tournament, Permissions = Permissions.Create });
            
            // Act
            getTournamentsOperationResult = await this.TournamentService.GetAsync(this.getOptions);
            
            // Assert
            this.AssertOperationResult(getTournamentsOperationResult, 2);
            
            // Act
            privateTournaments = this.CreateTournamentModels(4, privateTournaments: true);
            operationResult = await this.TournamentService.CreateManyAsync(privateTournaments);
            
            // Assert
            this.AssertOperationResult(operationResult, 4);
            
            // Act
            getTournamentsOperationResult = await this.TournamentService.GetAsync(this.getOptions);
            
            // Assert
            this.AssertOperationResult(getTournamentsOperationResult, 6);
            
            // Act
            publicTournaments = this.CreateTournamentModels(1);
            operationResult = await this.TournamentService.CreateManyAsync(publicTournaments);
            
            // Assert
            this.AssertOperationResult(operationResult, 1);
            
            // Act
            getTournamentsOperationResult = await this.TournamentService.GetAsync(this.getOptions);
            
            // Assert
            this.AssertOperationResult(getTournamentsOperationResult, 7);
            this.AuthenticateUser(firstUserId, new PermissionModel { Scope = PermissionScope.Tournament, Permissions = Permissions.Create });
            
            // Act
            getTournamentsOperationResult = await this.TournamentService.GetAsync(this.getOptions);
            
            // Assert
            this.AssertOperationResult(getTournamentsOperationResult, 8);
        }
        
        // Should not create more that default maximum
        
        private Tournament CreateTournamentModel(bool privateTournaments = false)
        {
            var tournament = new Tournament
            {
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Public = !privateTournaments,
            };

            return tournament;
        }
        
        private IEnumerable<Tournament> CreateTournamentModels(int count, bool privateTournaments = false)
        {
            for (var i = 0; i < count; i++)
                yield return this.CreateTournamentModel(privateTournaments);
        }
    }
}