using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using lvl.Web.Authorization;

namespace lvl.Oidc.AuthorizationServer.Seeder
{
    /// <summary>
    ///     Contains the logic for seeding related oidc data.
    /// </summary>
    /// <remarks>
    ///     This was placed into a class because the same logic was being repeated in multiple projects.
    /// </remarks>
    public class OidcAuthorizationServerSeeder
    {
        private ManditoryDataSeeder ManditoryDataSeeder { get; }
        private TestDataSeeder TestDataSeeder { get; }
        private OidcAuthorizationServerOptions Options { get; }
        private Impersonator Impersonator { get; }

        public OidcAuthorizationServerSeeder(ManditoryDataSeeder manditoryDataSeeder, TestDataSeeder testDataSeeder, OidcAuthorizationServerOptions options, Impersonator impersonator)
        {
            ManditoryDataSeeder = manditoryDataSeeder ?? throw new ArgumentNullException(nameof(manditoryDataSeeder));
            TestDataSeeder = testDataSeeder ?? throw new ArgumentNullException(nameof(testDataSeeder));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Impersonator = impersonator ?? throw new ArgumentNullException(nameof(impersonator));
        }

        /// <summary>
        ///     Populates the database if the configuration based on the configuration. This was added only to reduce the number of copy pastes of the same logic.
        /// </summary>
        /// <remarks>Will populate only manditory data only if the configuration SeedManditoryData is set.</remarks>
        /// <remarks>Will populate only test data only if the configuration SeedTestData is set.</remarks>
        public async Task SeedAsync()
        {
            Impersonator.AsAdministrator();

            if (Options.SeedManditoryData)
            {
                await ManditoryDataSeeder.SeedAsync();
            }

            if (Options.SeedTestData)
            {
                await TestDataSeeder.SeedAsync();
            }
        }
    }
}
