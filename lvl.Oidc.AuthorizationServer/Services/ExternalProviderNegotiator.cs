using IdentityServer4.Services;
using IdentityServer4.Stores;
using lvl.Oidc.AuthorizationServer.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Services
{
    public class ExternalProviderNegotiator
    {
        private IIdentityServerInteractionService InteractionService { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }
        private IClientStore ClientStore { get; }
        private OidcAuthorizationServerOptions Options { get; }

        public ExternalProviderNegotiator(IIdentityServerInteractionService interactionService, IHttpContextAccessor httpContextAccessor, IClientStore clientStore, OidcAuthorizationServerOptions options)
        {
            InteractionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            ClientStore = clientStore ?? throw new ArgumentNullException(nameof(clientStore));
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<IEnumerable<ExternalProvider>> GetProvidersAsync(string returnUrl)
        {
            var authorizationContext = await InteractionService.GetAuthorizationContextAsync(returnUrl);
            if (authorizationContext?.IdP != null)
            {
                return new[]
                {
                    new ExternalProvider { AuthenticationScheme = authorizationContext.IdP }
                };
            }

            var schemes = HttpContextAccessor.HttpContext.Authentication.GetAuthenticationSchemes();
            var windowsProvider = Options
                .WindowsProviders
                .Where(wp => schemes.Any(s => s.AuthenticationScheme.Equals(wp.AuthenticationScheme, StringComparison.OrdinalIgnoreCase)));

            var oauthProviders = schemes
                .Where(scheme => scheme.DisplayName != null)
                .Select(scheme => new ExternalProvider { AuthenticationScheme = scheme.AuthenticationScheme, DisplayName = scheme.DisplayName });

            var providers = windowsProvider.Union(oauthProviders);

            // if the client has restrictions, apply filters on how it can log in
            if(authorizationContext?.ClientId != null)
            {
                var client = await ClientStore.FindEnabledClientByIdAsync(authorizationContext.ClientId);
                if(client?.IdentityProviderRestrictions?.Any() == true)
                {
                    // filter the authentication scheme.
                    return providers.Where(p => client.IdentityProviderRestrictions.Contains(p.AuthenticationScheme));
                }
            }

            return providers;
        }
    }
}
