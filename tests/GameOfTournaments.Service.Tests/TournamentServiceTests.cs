namespace GameOfTournaments.Service.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Factories.Models;
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
            await this.AuthenticateUserWithTournamentsCreationPermissionsAsync();
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
            await this.AuthenticateUserWithTournamentsCreationPermissionsAsync();
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
            await this.AuthenticateUserWithTournamentsCreationPermissionsAsync();
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
            await this.AuthenticateUserWithTournamentsCreationPermissionsAsync();
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
            await this.AuthenticateUserWithTournamentsCreationPermissionsAsync();
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
            await this.AuthenticateUserWithTournamentsCreationPermissionsAsync();
            
            var tournaments = this.CreateTournamentModels(5, privateTournaments: true);
            var operationResult = await this.TournamentService.CreateManyAsync(tournaments);
            
            // Assert
            this.AssertOperationResult(operationResult, 5);

            // Act
            var getPrivateTournamentsOperationResult = await this.TournamentService.GetAsync(this.getOptions);
            this.AssertOperationResult(getPrivateTournamentsOperationResult, 5);

            // Switch user
            await this.AuthenticateUserWithTournamentsCreationPermissionsAsync();
            
            getPrivateTournamentsOperationResult = await this.TournamentService.GetAsync(this.getOptions);
            
            // Assert
            this.AssertOperationResult(getPrivateTournamentsOperationResult, 0);
        }
        
        [Fact]
        public async Task CreatingAndRetrievingShouldWorkAppropriatelyForUsers()
        {
            // Arrange
            var firstUserId = await this.AuthenticateUserWithTournamentsCreationPermissionsAsync();
            
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
            await this.AuthenticateUserWithTournamentsCreationPermissionsAsync();
            
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

        [Fact]
        public async Task ShouldCreateSuccessfullyUpToMaximumDefinedTournamentsPerDay()
        {
            // Arrange
            await this.AuthenticateUserWithTournamentsCreationPermissionsAsync(100);
            
            var privateTournaments = this.CreateTournamentModels(50, privateTournaments: true);
            var operationResult = await this.TournamentService.CreateManyAsync(privateTournaments);
            
            // Assert
            this.AssertOperationResult(operationResult, 50);
            
            var getTournamentsOperationResult = await this.TournamentService.GetAsync(this.getOptions);
            
            // Assert
            this.AssertOperationResult(getTournamentsOperationResult, 50);
        }
        
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

        private async Task<int> AuthenticateUserWithTournamentsCreationPermissionsAsync(int maximumTournamentsPerDay = 10)
        {
            var createApplicationUserOperationResult =
                await this.ApplicationUserService.CreateAsync(new RegisterUserModel { Username = Guid.NewGuid().ToString(), Password = Guid.NewGuid().ToString() });
            Assert.True(createApplicationUserOperationResult.Success);

            var id = this.AuthenticateUser(new PermissionModel { Scope = PermissionScope.Tournament, Permissions = Permissions.Create });
            
            var createApplicationUserAccountOperationResult = await this.ApplicationUserAccountService.CreateAsync(
                new ApplicationUserAccount { Id = id, ApplicationUserId = id, CreateTournamentsPerDay = maximumTournamentsPerDay });
            Assert.True(createApplicationUserAccountOperationResult.Success);

            return id;
        }
    }
}