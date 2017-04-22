using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace lvl.Oidc.AccessTokens.ResourceServer
{
    public class ResourceServerOptions
    {
        public string Authority { get; set; }
        public bool RequireHttpsMetadata { get; set; } = true;
        public string ApiName { get; set; }
        public string ApiSecret { get; set; }

        public ResourceServerOptions() { }

        public ResourceServerOptions(IConfiguration configuration)
        {
            configuration.GetSection("resource-server").Bind(this);
        }

        public IdentityServerAuthenticationOptions ToIdentityServer()
        {
            return new IdentityServerAuthenticationOptions
            {
                Authority = Authority,
                RequireHttpsMetadata = RequireHttpsMetadata,
                ApiName = ApiName,
                ApiSecret = ApiSecret
            };
        }
    }
}
