using IdentityServer4.Services;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Services
{
    public class CorsPolicyService : ICorsPolicyService
    {
        private IRepository<ClientEntity> ClientRepository { get; }
        private ILogger<CorsPolicyService> Logger { get; }

        public CorsPolicyService(IRepository<ClientEntity> clientRepository, ILogger<CorsPolicyService> logger)
        {
            ClientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            if (origin == null)
            {
                throw new ArgumentNullException(nameof(origin));
            }

            var clients = await ClientRepository.GetAsync();

            var isAllowed = clients.Any(client => client.AllowsOrigin(origin));

            if (!isAllowed)
            {
                Logger.LogError($"Origin '{origin}' is not allowed on any clients.");
            }

            return isAllowed;
        }
    }
}
