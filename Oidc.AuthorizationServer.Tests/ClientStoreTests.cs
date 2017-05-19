using IdentityServer4.Stores;
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
    public class ClientStoreTests
    {
        private IRepository<ClientEntity> ClientRepository { get; }
        private IClientStore ClientStore { get; }

        public ClientStoreTests(ServiceProviderFixture serviceProviderFixture)
        {
            var serviceProvider = serviceProviderFixture.ServiceProvider;
            ClientRepository = serviceProvider.GetRequiredService<IRepository<ClientEntity>>();
            ClientStore = serviceProvider.GetRequiredService<IClientStore>();
        }

        [Fact]
        public async Task It_will_throw_argument_null_exception_if_client_id_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => ClientStore.FindClientByIdAsync(null));
        }

        [Fact]
        public async Task It_will_throw_invalid_operation_exception_if_no_client_is_found()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => ClientStore.FindClientByIdAsync("non-existant-client"));
        }

        [Fact]
        public async Task It_will_return_client_when_id_is_matching()
        {
            await ClientRepository.CreateAsync(new ClientEntity {
                ClientId = "matching-client",
                ClientName = "Matching Client",
                ClientUri = "localhost"
            });

            var matchingClient = await ClientStore.FindClientByIdAsync("matching-client");

            Assert.NotNull(matchingClient);
        }
    }
}
