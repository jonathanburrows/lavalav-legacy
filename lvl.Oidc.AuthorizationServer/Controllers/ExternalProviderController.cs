using lvl.Oidc.AuthorizationServer.Services;
using lvl.Oidc.AuthorizationServer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Controllers
{
    [Route("oidc/external-providers")]
    public class ExternalProviderController : ControllerBase
    {
        private ExternalProviderNegotiator ExternalProviderNegotiator { get; }

        public ExternalProviderController(ExternalProviderNegotiator externalProviderNegotiator)
        {
            ExternalProviderNegotiator = externalProviderNegotiator ?? throw new ArgumentNullException(nameof(externalProviderNegotiator));
        }

        [HttpGet]
        public async Task<IEnumerable<ExternalProvider>> GetProvidersForReturnUrlAsync(string returnUrl)
        {
            return await ExternalProviderNegotiator.GetProvidersAsync(returnUrl);
        }
    }
}
