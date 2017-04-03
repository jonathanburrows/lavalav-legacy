using Microsoft.Extensions.Configuration;

namespace lvl.Oidc.AuthorizationServer
{
    public class OidcAuthorizationServerOptions
    {
        public OidcAuthorizationServerOptions() { }

        public OidcAuthorizationServerOptions(IConfiguration configuration)
        {
            configuration.GetSection("oidc:authorization-server").Bind(this);
        }
    }
}
