using IdentityServer4.Models;
using IdentityServer4.Stores;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Repositories;
using lvl.Repositories.Querying;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Stores
{
    public class ClientStore : IClientStore
    {
        private IRepository<ClientEntity> ClientRepository { get; }

        public ClientStore(IRepository<ClientEntity> clientRepository)
        {
            ClientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var clientQuery = new Query<ClientEntity>().Where(client => client.ClientId == clientId);
            var clients = await ClientRepository.GetAsync(clientQuery);
            if (clients.Count == 0)
            {
                throw new InvalidOperationException($"No clients with id {clientId} were found.");
            }
            if (clients.Count > 1)
            {
                throw new InvalidOperationException($"{clients.Count} clients were found with id {clientId}.");
            }

            return clients.Items.Single().ToIdentityClient();
        }
    }
}
