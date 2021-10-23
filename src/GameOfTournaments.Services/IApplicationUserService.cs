namespace GameOfTournaments.Services
{
    using System.Threading.Tasks;
    using GameOfTournaments.Data.Factories.Models;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Data.Models;

    public interface IApplicationUserService
    {
        Task<IOperationResult<ApplicationUser>> CreateAsync(RegisterUserModel registerUserModel);
    }
}