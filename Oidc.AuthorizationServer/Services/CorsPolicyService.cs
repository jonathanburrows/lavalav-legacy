using IdentityServer4.Services;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Services
{
    /// <summary>
    ///     Provides ability to check if any clients accept requests from a site.
    /// </summary>
    internal class CorsPolicyService : ICorsPolicyService
    {
        private IRepository<ClientEntity> ClientRepository { get; }

        public CorsPolicyService(IRepository<ClientEntity> clientRepository)
        {
            ClientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
        }

        /// <summary>
        ///     Checks if any client accepts requests from a given site.
        /// </summary>
        /// <param name="origin">The site which is making a request.</param>
        /// <exception cref="ArgumentNullException"><paramref name="origin"/> is null.</exception>
        /// <exception cref="InvalidOperationException">No client accepts the given origin.</exception>
        /// <returns>
        ///     Always true.
        ///     If no clients accept the origin, an exception is thrown.
        /// </returns>
        /// <remarks>
        ///     An exception is thrown when no clients accept the origin because it later gives a misleading error otherwise.
        /// </remarks>
        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            if (origin == null)
            {
                throw new ArgumentNullException(nameof(origin));
            }

            var clients = await ClientRepository.GetAsync();

            if (!clients.Any(client => client.AllowsOrigin(origin)))
            {
                // An exception is thrown because identity server gives a misleading error otherwise, and its not intuitive to check the log files.
                throw new InvalidOperationException($"Origin '{origin}' is not allowed on any clients.");
            }

            // true is always returned because if false, an exception is thrown.
            return true;
        }
    }
}
