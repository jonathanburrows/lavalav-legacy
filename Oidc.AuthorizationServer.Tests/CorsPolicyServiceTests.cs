using IdentityServer4.Services;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Oidc.AuthorizationServer.Tests.Fixtures;
using lvl.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace lvl.Oidc.AuthorizationServer.Tests
{
    [Collection(nameof(OidcAuthorizationServerCollection))]
    public class CorsPolicyServiceTests
    {
        private IRepository<ClientEntity> ClientRepository { get; }
        private ICorsPolicyService CorsPolicyService { get; }

        public CorsPolicyServiceTests(ServiceProviderFixture serviceProviderFixture)
        {
            var serviceProvider = serviceProviderFixture.ServiceProvider;
            ClientRepository = serviceProvider.GetRequiredService<IRepository<ClientEntity>>();
            CorsPolicyService = serviceProvider.GetRequiredService<ICorsPolicyService>();
        }

        [Fact]
        public async Task It_will_throw_argument_null_exception_when_origin_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => CorsPolicyService.IsOriginAllowedAsync(null));
        }

        [Fact]
        public async Task It_will_throw_invalid_operation_exception_when_no_clients()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => CorsPolicyService.IsOriginAllowedAsync("no-clients"));
        }

        [Fact]
        public async Task It_will_throw_invalid_operation_exception_when_clients_that_dont_support_origin()
        {
            var client = new ClientEntity
            {
                ClientId = "no-matching-client",
                ClientName = "client",
                ClientUri = "localhost",
                AllowedCorsOrigins = { new CorsOrigin { Name = "no-matching-client" } }
            };
            await ClientRepository.CreateAsync(client);

            await Assert.ThrowsAsync<InvalidOperationException>(() => CorsPolicyService.IsOriginAllowedAsync("mis-matched"));
        }

        [Fact]
        public async Task It_will_return_true_when_single_client_matching_origin()
        {
            var client = new ClientEntity
            {
                ClientId = "single-matching-client",
                ClientName = "client",
                ClientUri = "localhost",
                AllowedCorsOrigins = { new CorsOrigin { Name = "single-matching-client" } }
            };
            await ClientRepository.CreateAsync(client);

            var isAllowedOrigin = await CorsPolicyService.IsOriginAllowedAsync("single-matching-client");
            Assert.True(isAllowedOrigin);
        }

        [Fact]
        public async Task It_will_return_true_when_only_one_clients_matching_origin()
        {
            await ClientRepository.CreateAsync(new ClientEntity
            {
                ClientId = "one-matching-client1",
                ClientUri = "localhost",
                ClientName = "client",
                AllowedCorsOrigins = { new CorsOrigin { Name = "one-matching-client" } }
            });
            await ClientRepository.CreateAsync(new ClientEntity
            {
                ClientId = "one-matching-client2",
                ClientName = "client",
                ClientUri = "localhost",
                AllowedCorsOrigins = { new CorsOrigin { Name = "mismatched" } }
            });

            var isAllowedOrigin = await CorsPolicyService.IsOriginAllowedAsync("one-matching-client");
            Assert.True(isAllowedOrigin);
        }
    }
}
