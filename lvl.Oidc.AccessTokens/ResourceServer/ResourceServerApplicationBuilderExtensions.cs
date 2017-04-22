using lvl.Oidc.AccessTokens.ResourceServer;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class ResourceServerApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseResourceServer(this IApplicationBuilder applicationBuilder, ResourceServerOptions options)
        {
            if(applicationBuilder == null)
            {
                throw new ArgumentNullException();
            }

            if(options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.Authority == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.Authority)} is null.");
            }
            if (options.ApiName == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.ApiName)} is null.");
            }
            if (options.ApiSecret == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.ApiSecret)} is null.");
            }

            return applicationBuilder.UseIdentityServerAuthentication(options.ToIdentityServer());
        }
    }
}
