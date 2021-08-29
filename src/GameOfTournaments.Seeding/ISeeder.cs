namespace GameOfTournaments.Seeder
{
    using System.Threading.Tasks;

    public interface ISeeder
    {
        Task<bool> AlreadySeededAsync();
        
        Task SeedAsync();
    }
}