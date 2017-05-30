using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Oidc.AuthorizationServer.Stores;
using lvl.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Seeder
{
    /// <summary>
    ///     A utility to add data which is used for testing and development.
    /// </summary>
    public class TestDataSeeder
    {
        private UserStore UserStore { get; }
        private IRepository<ApiResourceEntity> ApiResourceRepository { get; }
        private IRepository<ClientEntity> ClientRepository { get; }

        public TestDataSeeder(UserStore userStore, IRepository<ApiResourceEntity> apiResourceRepository, IRepository<ClientEntity> clientRepository)
        {
            UserStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            ApiResourceRepository = apiResourceRepository ?? throw new ArgumentNullException(nameof(apiResourceRepository));
            ClientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
        }

        /// <summary>
        ///     Will populate the database with data used only for testing and development.
        /// </summary>
        /// <remarks>
        ///     This method will be a bit long, and thats ok! Its a dump of data, be a bit liberal.
        /// </remarks>
        public async Task SeedAsync()
        {
            // Seeding users
            var testUsers = new[]
            {
                new User
                {
                    SubjectId = "testuser@lavalav.com",
                    Username = "testuser",
                    HashedPassword = "password",
                    Claims = new []
                    {
                        new ClaimEntity{ Type = JwtClaimTypes.Name, Value = "jonathan burrows" },
                        new ClaimEntity{ Type = JwtClaimTypes.Email, Value = "testuser@lavalav.com" }
                    }
                },
                new User
                {
                    SubjectId = "emailless-user",
                    Username = "emailless-user",
                    HashedPassword = "password",
                    Claims = new []
                    {
                        new ClaimEntity{ Type = JwtClaimTypes.Name, Value = "jonathan burrows" }
                    }
                }
            };

            var missingUsers = testUsers.Where(user => UserStore.FindByUsernameAsync(user.Username).Result == null);
            foreach (var missingUser in missingUsers)
            {
                await UserStore.AddUserAsync(missingUser);
            }

            // Seeding api resources.
            var apiResources = new[]
            {
                new ApiResourceEntity("test-resource-server", "Test Resource Server")
            };

            var existingResources = await ApiResourceRepository.GetAsync();
            var missingResources = apiResources.Where(required => existingResources.All(existing => required.Name != existing.Name));

            foreach (var missingResource in missingResources)
            {
                await ApiResourceRepository.CreateAsync(missingResource);
            }

            // seeding clients.
            var clients = new[]
            {
                new ClientEntity
                {
                    ClientId = "test-implicit-client",
                    ClientName = "Test implicit client",
                    ClientUri = "http://localhost:0000",
                    ClientSecrets = new [] { new SecretEntity { Value = "secret".Sha256() } },
                    AllowedGrantTypes = GrantTypes.Implicit.Select(gt => new GrantTypeEntity { Name = gt }).ToList(),

                    RequireConsent = true,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AlwaysSendClientClaims = true,
                    UpdateAccessTokenClaimsOnRefresh = true,

                    RedirectUris = new []{ new RedirectUri { Name = "http://localhost:0000" } },
                    PostLogoutRedirectUris = new []{ new PostLogoutRedirectUri { Name = "http://localhost:0000" } },
                    AllowedCorsOrigins = new []{ new CorsOrigin { Name = "http://localhost:0000" } },
                    LogoutUri = "http://localhost:0000",
                    AllowedScopes = new []
                    {
                        new AllowedScope { Name = "test-resource-server" },
                        new AllowedScope { Name = IdentityServerConstants.StandardScopes.OpenId },
                        new AllowedScope { Name = IdentityServerConstants.StandardScopes.Profile }
                    }
                },
                new ClientEntity
                {
                    ClientId = "test-resource-owner-client",
                    ClientName = "Test Resource Owner Client",
                    ClientSecrets = new [] { new SecretEntity { Value = "secret".Sha256() } },
                    ClientUri = "http://localhost:5005",
                    AllowedGrantTypes = new []{ GrantType.ResourceOwnerPassword, GrantType.AuthorizationCode } .Select(gt => new GrantTypeEntity { Name = gt }).ToList(),

                    RequireConsent = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,

                    RedirectUris = new []{ new RedirectUri { Name = "http://localhost:5005" } },
                    PostLogoutRedirectUris = new []{ new PostLogoutRedirectUri { Name = "http://localhost:5005" } },
                    AllowedCorsOrigins = new []{
                        new CorsOrigin { Name = "http://localhost:5005" },
                        new CorsOrigin { Name = "http://localhost:5006" },
                        new CorsOrigin { Name = "http://localhost:5007" }
                    },
                    LogoutUri = "http://localhost:5005",
                    AllowedScopes = new []
                    {
                        new AllowedScope { Name = "test-resource-server" },
                        new AllowedScope { Name = IdentityServerConstants.StandardScopes.OpenId },
                        new AllowedScope { Name = IdentityServerConstants.StandardScopes.OfflineAccess},
                        new AllowedScope { Name = IdentityServerConstants.StandardScopes.Profile }
                    }
                }
            };

            var existingClients = await ClientRepository.GetAsync();
            var missingClients = clients.Where(required => existingClients.All(existing => required.ClientId != existing.ClientId));

            foreach (var missingClient in missingClients)
            {
                await ClientRepository.CreateAsync(missingClient);
            }
        }
    }
}
