using IdentityServer4.Stores;
using IdentityServer4.Validation;
using lvl.Oidc.AuthorizationServer;
using lvl.Oidc.AuthorizationServer.Seeder;
using lvl.Oidc.AuthorizationServer.Services;
using lvl.Oidc.AuthorizationServer.Stores;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.FileProviders;
using System;
using System.Reflection;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using IdentityServer4.Configuration;

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

            Action<IdentityServerOptions> identityServerOptions = o =>
            {
                o.UserInteraction.LoginUrl = "/oidc/login";
                o.UserInteraction.LogoutUrl = "/oidc/logout";
                o.UserInteraction.ConsentUrl = "/oidc/consent";
            };

            serviceCollection
                .AddIdentityServer(identityServerOptions)
                .AddClientStore<ClientStore>()
                .AddClientStoreCache<ClientStore>()
                .AddResourceStore<ResourceStore>()
                .AddResourceStoreCache<ResourceStore>()
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddTemporarySigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryCaching();

            var assembly = typeof(OidcAuthorizationServerServiceCollectionExtensions).GetTypeInfo().Assembly;
            var embeddedFileProvider = new EmbeddedFileProvider(assembly);

            serviceCollection
                .AddAuthorization()
                .Configure<RazorViewEngineOptions>(o => o.FileProviders.Add(embeddedFileProvider))
                .AddScoped<PasswordHasher>()
                .AddScoped<IClientStore, ClientStore>()
                .AddScoped<IPersistedGrantStore, PersistedGrantStore>()
                .AddScoped<IResourceStore, ResourceStore>()
                .AddScoped<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>()
                .AddScoped<IActionContextAccessor, ActionContextAccessor>()
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
