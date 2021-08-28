namespace GameOfTournaments.Services
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string username, string secret);
    }
}