using IdentityServer4.Models;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace lvl.Oidc.Seeder
{
    internal class ManditoryDataSeeder
    {
        private IRepository<IdentityResourceEntity> IdentityResourceRepository { get; }

        public ManditoryDataSeeder(IRepository<IdentityResourceEntity> identityResourceRepository)
        {
            IdentityResourceRepository = identityResourceRepository ?? throw new ArgumentNullException(nameof(identityResourceRepository));
        }

        public async Task Seed()
        {
            await SeedIdentityResourcesAsync();
        }

        private async Task SeedIdentityResourcesAsync()
        {
            var identityResources = new[]
            {
                IdentityResourceEntity.FromIdentityServer(new IdentityResources.OpenId()),
                IdentityResourceEntity.FromIdentityServer(new IdentityResources.Profile())
            };

            var existingIdentityResources = await IdentityResourceRepository.GetAsync();
            var missingIdentityResources = identityResources.Where(required => !existingIdentityResources.Any(existing => required.Name == existing.Name));

            foreach (var missingIdentityResource in missingIdentityResources)
            {
                await IdentityResourceRepository.CreateAsync(missingIdentityResource);
            }
        }
    }
}
