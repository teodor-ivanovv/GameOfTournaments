namespace GameOfTournaments.Services
{
    using System.Threading.Tasks;
    using GameOfTournaments.Services.Infrastructure;

    public interface IAuthenticationService
    {
        IAuthenticationContext Context { get; set; }
        
        bool Authenticated { get; }

        void Set(IAuthenticationContext context);

        Task<bool> IsInRoleAsync(string role);
    }
}