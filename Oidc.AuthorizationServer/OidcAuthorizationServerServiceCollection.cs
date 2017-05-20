using lvl.Oidc.AuthorizationServer.Services;
using lvl.Oidc.AuthorizationServer.Stores;
using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using lvl.Oidc.AuthorizationServer.Seeder;
using IdentityServer4.Stores;

// ReSharper disable once CheckNamespace In compliance with Microsoft's extension convention.
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Provides a method to register all services used by the oidc authorization server.
    /// </summary>
    public static class OidcAuthorizationServerServiceCollectionExtensions
    {
        /// <summary>
        ///     Registers all services used by oidc authorization server.
        /// </summary>
        /// <param name="serviceCollection">The service collection which will have services registered against it.</param>
        /// <param name="options">The options used by the oidc server.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> is null.</exception>
        /// <returns>The service collection with all services registered.</returns>
        public static IServiceCollection AddOidcAuthorizationServer(this IServiceCollection serviceCollection, OidcAuthorizationServerOptions options = null)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection
                .AddIdentityServer(o =>
                {
                    o.UserInteraction.LoginUrl = "/oidc/login";
                    o.UserInteraction.LogoutUrl = "/oidc/logout";
                    o.UserInteraction.ConsentUrl = "/oidc/consent";
                })
                .AddTemporarySigningCredential()
                .AddInMemoryCaching()
                .AddCorsPolicyService<CorsPolicyService>()
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddClientStore<ClientStore>();

            serviceCollection
                .AddAuthorization()
                .AddScoped<PasswordHasher>()
                .AddScoped<IActionContextAccessor, ActionContextAccessor>()
                .AddScoped<ManditoryDataSeeder>()
                .AddScoped<TestDataSeeder>()
                .AddScoped<OidcAuthorizationServerSeeder>()
                .AddScoped<UserStore>()
                .AddScoped<IPersistedGrantStore, PersistedGrantStore>()
                .AddSingleton(options ?? new OidcAuthorizationServerOptions());

            return serviceCollection;
        }
    }
}
