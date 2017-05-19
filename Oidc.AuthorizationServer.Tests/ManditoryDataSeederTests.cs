using lvl.Oidc.AuthorizationServer.Models;
using lvl.Oidc.AuthorizationServer.Seeder;
using lvl.Oidc.AuthorizationServer.Tests.Fixtures;
using lvl.Repositories;
using lvl.Repositories.Querying;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace lvl.Oidc.AuthorizationServer.Tests
{
    [Collection(nameof(OidcAuthorizationServerCollection))]
    public class ManditoryDataSeederTests
    {
        private ManditoryDataSeeder ManditoryDataSeeder { get; }
        private IRepository<IdentityResourceEntity> IdentityResourceRepository { get; }

        public ManditoryDataSeederTests(ServiceProviderFixture serviceProviderFixture)
        {
            var serviceProvider = serviceProviderFixture.ServiceProvider;
            ManditoryDataSeeder = serviceProvider.GetRequiredService<ManditoryDataSeeder>();
            IdentityResourceRepository = serviceProvider.GetRequiredService<IRepository<IdentityResourceEntity>>();
        }

        [Fact]
        public async Task It_will_add_open_id_identity_resource() {
            await ManditoryDataSeeder.SeedAsync();

            var openIdQuery = new Query<IdentityResourceEntity>().Where(ir => ir.Name == "openid");
            var openIds = await IdentityResourceRepository.GetAsync(openIdQuery);
            Assert.Equal(openIds.Count, 1);
        }

        [Fact]
        public async Task It_will_add_provide_identity_resource()
        {
            await ManditoryDataSeeder.SeedAsync();

            var profileQuery = new Query<IdentityResourceEntity>().Where(ir => ir.Name == "profile");
            var profiles = await IdentityResourceRepository.GetAsync(profileQuery);
            Assert.Equal(profiles.Count, 1);
        }
    }
}
