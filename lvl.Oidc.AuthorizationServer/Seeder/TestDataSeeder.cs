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
    internal class TestDataSeeder
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

        public async Task SeedAsync()
        {
            await SeedUsersAsync();
            await SeedApiResourcesAsync();
            await SeedClientsAsync();
        }

        private async Task SeedUsersAsync()
        {
            var testUsers = new[]
            {
                new User
                {
                    SubjectId = "testuser@lavalav.com",
                    Username = "testuser",
                    HashedPassword = "password",
                    Claims = new []
                    {
                        new ClaimEntity{ Type = "name", Value = "jonathan burrows" }
                    }
                }
            };

            foreach (var user in testUsers)
            {
                await UserStore.AddUserAsync(user);
            }
        }

        private async Task SeedApiResourcesAsync()
        {
            var apiResources = new[]
            {
                new ApiResourceEntity { Name = "resource-server", Description = "Resource Server" }
            };

            var existingResources = await ApiResourceRepository.GetAsync();
            var missingResources = apiResources.Where(required => !existingResources.Any(existing => required.Name == existing.Name));

            foreach (var missingResource in missingResources)
            {
                await ApiResourceRepository.CreateAsync(missingResource);
            }
        }

        private async Task SeedClientsAsync()
        {
            var clients = new[]
            {
                new ClientEntity
                {
                    ClientId = "test-implicit-client",
                    ClientName = "Test implicit client",
                    ClientSecrets = new [] { new SecretEntity { Value = "secret".Sha256() } },
                    AllowedGrantTypes = GrantTypes.Implicit.Select(gt => new GrantTypeEntity { Name = gt }),

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
                    ClientUri = "http://localhost:4200",
                    AllowedGrantTypes = new []{ GrantType.ResourceOwnerPassword, GrantType.AuthorizationCode } .Select(gt => new GrantTypeEntity { Name = gt }),

                    RequireConsent = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,

                    RedirectUris = new []{ new RedirectUri { Name = "http://localhost:4200" } },
                    PostLogoutRedirectUris = new []{ new PostLogoutRedirectUri { Name = "http://localhost:4200" } },
                    AllowedCorsOrigins = new []{ new CorsOrigin { Name = "http://localhost:4200" } },
                    LogoutUri = "http://localhost:4200",
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
            var missingClients = clients.Where(required => !existingClients.Any(existing => required.ClientId == existing.ClientId));

            foreach (var missingClient in missingClients)
            {
                await ClientRepository.CreateAsync(missingClient);
            }
        }
    }
}
