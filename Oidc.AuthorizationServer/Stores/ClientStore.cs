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
    /// <summary>
    ///     Retrieval of client configuration.
    /// </summary>
    internal class ClientStore : IClientStore
    {
        private IRepository<ClientEntity> ClientRepository { get; }

        public ClientStore(IRepository<ClientEntity> clientRepository)
        {
            ClientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
        }

        /// <summary>
        ///     Fetches a client with a matching id.
        /// </summary>
        /// <param name="clientId">The string identifier of the client, not to be confused with the database id.</param>
        /// <exception cref="ArgumentNullException"><paramref name="clientId"/> is null.</exception>
        /// <exception cref="InvalidOperationException">There was no client with the matching id.</exception>
        /// <exception cref="InvalidOperationException">There was more than one client with the given id.</exception>
        /// <returns>The matching client.</returns>
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            if (clientId == null)
            {
                throw new ArgumentNullException(nameof(clientId));
            }

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
