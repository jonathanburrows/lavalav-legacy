using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using lvl.Oidc.AuthorizationServer.Seeder;
using lvl.Oidc.AuthorizationServer.Services;
using lvl.Oidc.AuthorizationServer.Stores;
using lvl.Oidc.AuthorizationServer.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace lvl.Oidc.AuthorizationServer.Tests
{
    [Collection(nameof(OidcAuthorizationServerCollection))]
    public class ServiceCollectionExtensionTests
    {
        private IServiceProvider ServiceProvider { get; }

        public ServiceCollectionExtensionTests(ServiceProviderFixture serviceProviderFixture)
        {
            ServiceProvider = serviceProviderFixture.ServiceProvider;
        }

        [Fact]
        public void It_will_allow_cors_policy_service_to_be_resolved()
        {
            var corsPolicyService = ServiceProvider.GetRequiredService<ICorsPolicyService>();

            Assert.NotNull(corsPolicyService);
        }

        [Fact]
        public void It_will_allow_resource_owner_password_validator_to_be_resolved()
        {
            var resourceOwnerPasswordValidator = ServiceProvider.GetRequiredService<IResourceOwnerPasswordValidator>();

            Assert.NotNull(resourceOwnerPasswordValidator);
        }

        [Fact]
        public void It_will_allow_client_store_to_be_resolved()
        {
            var clientStore = ServiceProvider.GetRequiredService<IClientStore>();

            Assert.NotNull(clientStore);
        }

        [Fact]
        public void It_will_allow_resource_store_to_be_resolved()
        {
            var resourceStore = ServiceProvider.GetRequiredService<IResourceStore>();

            Assert.NotNull(resourceStore);
        }

        [Fact]
        public void It_will_allow_password_hasher_to_be_resolved()
        {
            var passwordHasher = ServiceProvider.GetRequiredService<PasswordHasher>();

            Assert.NotNull(passwordHasher);
        }

        [Fact]
        public void It_will_allow_seeder_to_be_resolved()
        {
            var seeder = ServiceProvider.GetRequiredService<OidcAuthorizationServerSeeder>();

            Assert.NotNull(seeder);
        }

        [Fact]
        public void It_will_allow_user_store_to_be_resolved()
        {
            var userStore = ServiceProvider.GetRequiredService<UserStore>();

            Assert.NotNull(userStore);
        }

        [Fact]
        public void It_will_allow_persisted_grant_store_to_be_resolved()
        {
            var persistedGrantStore = ServiceProvider.GetRequiredService<IPersistedGrantStore>();

            Assert.NotNull(persistedGrantStore);
        }

        [Fact]
        public void It_will_allow_oidc_authorization_server_options_to_be_resolved()
        {
            var options = ServiceProvider.GetRequiredService<OidcAuthorizationServerOptions>();

            Assert.NotNull(options);
        }
    }
}
