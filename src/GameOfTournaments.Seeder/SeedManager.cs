namespace GameOfTournaments.Seeder
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;

    public class SeedManager : ISeedManager
    {
        public async Task SeedAsync(IEnumerable<ISeeder> seeders)
        {
            var enumerated = seeders?.ToList();
            Guard.Against.Null(enumerated, nameof(seeders));

            foreach (var seeder in enumerated)
                await seeder.SeedAsync();
        }
    }
}