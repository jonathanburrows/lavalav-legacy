using IdentityModel;
using IdentityServer4.Models;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Seeder
{
    /// <summary>
    ///     Utility to add all data required for the authorization server to run.
    /// </summary>
    public class OidcManditoryDataSeeder
    {
        private IRepository<IdentityResourceEntity> IdentityResourceRepository { get; }

        public OidcManditoryDataSeeder(IRepository<IdentityResourceEntity> identityResourceRepository)
        {
            IdentityResourceRepository = identityResourceRepository ?? throw new ArgumentNullException(nameof(identityResourceRepository));
        }

        /// <summary>
        ///     Will add all data required for the autorization server to run.
        /// </summary>
        public async Task SeedAsync()
        {
            var identityResources = new[]
            {
                IdentityResourceEntity.FromIdentityServer(new IdentityResources.OpenId()),
                IdentityResourceEntity.FromIdentityServer(new IdentityResources.Profile()),
                IdentityResourceEntity.FromIdentityServer(new IdentityResources.Email()),
                new IdentityResourceEntity
                {
                    Name = JwtClaimTypes.Role,
                    DisplayName = "Roles",
                    UserClaims = new []{ new UserClaim { Name = JwtClaimTypes.Role} }
                }
            };

            var existingIdentityResources = await IdentityResourceRepository.GetAsync();
            var missingIdentityResources = identityResources.Where(required => existingIdentityResources.All(existing => required.Name != existing.Name));

            foreach (var missingIdentityResource in missingIdentityResources)
            {
                await IdentityResourceRepository.CreateAsync(missingIdentityResource);
            }
        }
    }
}
