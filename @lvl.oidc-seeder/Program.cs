using Microsoft.Extensions.DependencyInjection;

namespace lvl.Oidc.Seeder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var argumentParser = new ArgumentParser();
            var options = argumentParser.Parse(args);

            var serviceProvider = new ServiceCollection()
                .AddDomains(options.ConnectionString)
                .AddDatabaseGeneration()
                .AddRepositories()
                .AddWeb()
                .AddOidcAuthorizationServer()
                .AddOidcSeeder()
                .BuildServiceProvider();

            var manditoryDataSeeder = serviceProvider.GetRequiredService<ManditoryDataSeeder>();
            manditoryDataSeeder.Seed().Wait();

            if (options.SeedTestData)
            {
                var testDataSeeder = serviceProvider.GetRequiredService<TestDataSeeder>();
                testDataSeeder.Seed().Wait();
            }
        }
    }
}