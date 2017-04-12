using System.Collections.Generic;

namespace lvl.Oidc.AuthorizationServer.ViewModels
{
    public class ConsentViewModel
    {
        public string ClientName { get; set; }
        public string ClientUrl { get; set; }
        public string ClientLogoUrl { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberConsent { get; set; }
        public bool Confirmed { get; set; }

        public IEnumerable<string> ScopesConsented { get; set; }
        public IEnumerable<ScopeModel> IdentityScopes { get; set; }
        public IEnumerable<ScopeModel> ResourceScopes { get; set; }
    }
}
