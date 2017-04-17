using IdentityServer4.Stores;
using IdentityServer4.Validation;
using lvl.Oidc.AuthorizationServer;
using lvl.Oidc.AuthorizationServer.Seeder;
using lvl.Oidc.AuthorizationServer.Services;
using lvl.Oidc.AuthorizationServer.Stores;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OidcAuthorizationServerServiceCollectionExtensions
    {
        public static IServiceCollection AddOidcAuthorizationServer(this IServiceCollection serviceCollection, OidcAuthorizationServerOptions options = null)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection
                .AddIdentityServer()
                .AddClientStore<ClientStore>()
                .AddClientStoreCache<ClientStore>()
                .AddResourceStore<ResourceStore>()
                .AddResourceStoreCache<ResourceStore>()
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddTemporarySigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryCaching();

            serviceCollection
                .AddAuthorization()
                .AddScoped<PasswordHasher>()
                .AddScoped<IClientStore, ClientStore>()
                .AddScoped<IPersistedGrantStore, PersistedGrantStore>()
                .AddScoped<IResourceStore, ResourceStore>()
                .AddScoped<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>()
                .AddScoped<UserStore>()
                .AddScoped<ExternalProviderNegotiator>()
                .AddScoped<ArgumentParser>()
                .AddScoped<ManditoryDataSeeder>()
                .AddScoped<TestDataSeeder>()
                .AddSingleton(options ?? new OidcAuthorizationServerOptions());

            return serviceCollection;
        }
    }
}
