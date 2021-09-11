namespace GameOfTournaments.Services
{
    using GameOfTournaments.Services.Infrastructure;

    public interface IAuthenticationService
    {
        IAuthenticationContext Context { get; set; }
        
        bool Authenticated { get; }

        void Set(IAuthenticationContext context);

        bool IsInRole(string role);
    }
}