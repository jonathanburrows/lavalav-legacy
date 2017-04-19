using lvl.Oidc.AuthorizationServer.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace lvl.Oidc.AuthorizationServer
{
    public class OidcAuthorizationServerOptions
    {
        public bool SeedTestData { get; set; }
        public bool SeedManditoryData { get; set; }
        public IList<ExternalProvider> WindowsProviders { get; set; } = new List<ExternalProvider>();
        public ExternalApplicationInformation Facebook { get; set; }
        public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(30);
        public bool AutomaticRedirectionAfterSignOut { get; set; } = true;
        public bool ShowLogoutPrompt { get; set; }

        public OidcAuthorizationServerOptions() { }

        public OidcAuthorizationServerOptions(IConfiguration configuration)
        {
            configuration.GetSection("oidc:authorization-server").Bind(this);
        }
    }
}
