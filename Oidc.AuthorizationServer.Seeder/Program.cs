using lvl.DatabaseGenerator;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace lvl.Oidc.AuthorizationServer.Seeder
{
    /// <summary>
    ///     An executable which can call the seeder methods from Oidc.AuthorizationServer.
    /// </summary>
    internal class Program
    {
        /// <summary>
        ///     Will seed either manditory or test data into a given database.
        /// </summary>
        /// <param name="args">
        ///     Arguments supplied by user which will control logic of the seeding.
        ///
        ///     Syntax:
        ///         lvl.Oidc.AuthorizationServer.Seed --connection-string 'connection-string' [--seed-menditory] [-seed-test]
        ///
        ///     Options:
        ///         --connection-string: the database which will be populated with data.
        ///
        ///         --seed-manditory (optional): If marked, will add data which is critical for the application to run.
        ///
        ///         --seed-test (optional): If marked, will add data which can be used for tests and development.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="args"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Connection string is not provided.</exception>
        private static void Main(string[] args)
        {
            if(args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var argumentParser = new ArgumentParser();

            var connectionString = argumentParser.GetRequired<string>(args, "--connection-string");
            var domainOptions = new DomainOptions { ConnectionString = connectionString };

            var oidcAuthorizationServerOptions = new OidcAuthorizationServerOptions
            {
                SeedManditoryData = argumentParser.HasFlag(args, "--seed-manditory"),
                SeedTestData = argumentParser.HasFlag(args, "--seed-test")
            };

            var services = new ServiceCollection()
                .AddDomains(domainOptions)
                .AddDatabaseGeneration()
                .AddRepositories()
                .AddWeb()
                .AddOidcAuthorizationServer(oidcAuthorizationServerOptions)
                .BuildServiceProvider();

            var seeder = services.GetRequiredService<OidcAuthorizationServerSeeder>();
            seeder.SeedAsync().Wait();
        }
    }
}