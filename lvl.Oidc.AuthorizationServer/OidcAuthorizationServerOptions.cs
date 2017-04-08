using lvl.Oidc.AuthorizationServer.Seeder;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace lvl.Oidc.AuthorizationServer
{
    public class OidcAuthorizationServerOptions
    {
        public GenerationOptions GenerationOptions { get; set; }

        public OidcAuthorizationServerOptions() { }

        public OidcAuthorizationServerOptions(IConfiguration configuration)
        {
            configuration.GetSection("oidc:authorization-server").Bind(this);

            var connectionString = configuration.GetValue<string>("connection-string");
            GenerationOptions = configuration.Get<GenerationOptions>();
            GenerationOptions.ConnectionString = GenerationOptions.ConnectionString ?? connectionString;
        }
    }
}
