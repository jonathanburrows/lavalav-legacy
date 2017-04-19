using lvl.Oidc.AuthorizationServer.Seeder;
using lvl.Ontology;
using Microsoft.Extensions.DependencyInjection;

namespace lvl.Oidc.Seeder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var argumentParser = new ArgumentParser();
            var seedOptions = argumentParser.Parse(args);
            var connectionString = argumentParser.GetValue(args, "--connection-string");

            var serviceProvider = new ServiceCollection()
                .AddDomains(new DomainOptions { ConnectionString = connectionString })
                .AddDatabaseGeneration()
                .AddRepositories()
                .AddWeb()
                .AddOidcAuthorizationServer()
                .BuildServiceProvider();
            
            if(seedOptions.SeedManditoryData || seedOptions.SeedTestData)
            {
                var migrator = serviceProvider.GetRequiredService<DatabaseGenerator.DatabaseMigrator>();
                migrator.Migrate();
            }

            if (seedOptions.SeedManditoryData)
            {
                var manditoryDataSeeder = serviceProvider.GetRequiredService<ManditoryDataSeeder>();
                manditoryDataSeeder.SeedAsync().Wait();
            }

            if (seedOptions.SeedTestData)
            {
                var testDataSeeder = serviceProvider.GetRequiredService<TestDataSeeder>();
                testDataSeeder.SeedAsync().Wait();
            }
        }
    }
}