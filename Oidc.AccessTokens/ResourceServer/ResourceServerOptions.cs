using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;

// ReSharper disable once CheckNamespace In compliance with microsoft convention.
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     A fascade around the more complex IdentityServerAuthenticationOptions.
    /// </summary>
    public class ResourceServerOptions
    {
        /// <summary>
        ///     Address to the authorization server which can validate tokens.
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        ///     Will reject http requests if true.
        /// </summary>
        public bool RequireHttpsMetadata { get; set; } = true;

        /// <summary>
        ///     The name that this server is listed as on the authorization server.
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        ///     The secret authenticator for this server on the authorization server.
        /// </summary>
        public string ApiSecret { get; set; }

        public ResourceServerOptions() { }

        public ResourceServerOptions(IConfiguration configuration)
        {
            if(configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.GetSection("oidc:resource-server").Bind(this);
        }

        /// <summary>
        ///     Converts options into something identity server can use.
        /// </summary>
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
