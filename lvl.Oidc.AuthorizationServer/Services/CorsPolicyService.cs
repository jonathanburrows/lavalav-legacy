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
            var clients = await ClientRepository.GetAsync();
            var allowedOrigins = clients
                .SelectMany(client => client.AllowedCorsOrigins)
                .Where(allowedOrigin => allowedOrigin != null)
                .Distinct();

            return allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);
        }
    }
}
