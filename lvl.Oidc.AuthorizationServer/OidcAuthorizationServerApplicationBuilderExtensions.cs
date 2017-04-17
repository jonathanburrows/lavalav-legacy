using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class OidcAuthorizationServerApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseOidcAuthorizationServer(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
            {
                throw new ArgumentNullException(nameof(applicationBuilder));
            }

            return applicationBuilder
                .UseCors(options => options.AllowAnyOrigin())
                .UseCookieAuthentication()
                .UseIdentityServer();
        }
    }
}
