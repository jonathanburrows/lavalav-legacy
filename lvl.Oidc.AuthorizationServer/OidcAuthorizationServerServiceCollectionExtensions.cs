using IdentityServer4.Stores;
using lvl.Oidc.AuthorizationServer.Services;
using lvl.Oidc.AuthorizationServer.Stores;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OidcAuthorizationServerServiceCollectionExtensions
    {
        public static IServiceCollection AddOidcAuthorizationServer(this IServiceCollection serviceCollection)
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
                .AddTemporarySigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryCaching();

            serviceCollection
                .AddScoped<PasswordHasher>()
                .AddScoped<IClientStore, ClientStore>()
                .AddScoped<IPersistedGrantStore, PersistedGrantStore>()
                .AddScoped<IResourceStore, ResourceStore>()
                .AddScoped<UserStore>();

            return serviceCollection;
        }
    }
}
