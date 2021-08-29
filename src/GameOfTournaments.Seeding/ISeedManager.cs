namespace GameOfTournaments.Seeder
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISeedManager
    {
        Task SeedAsync(IEnumerable<ISeeder> seeders);
    }
}