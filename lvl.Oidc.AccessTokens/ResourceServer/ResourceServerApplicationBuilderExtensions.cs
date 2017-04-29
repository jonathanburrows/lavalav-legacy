using lvl.Oidc.AccessTokens.ResourceServer;
using lvl.Web.Cors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Microsoft.AspNetCore.Builder
{
    public static class ResourceServerApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseResourceServer(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
            {
                throw new ArgumentNullException();
            }

            var resourceServerOptions = applicationBuilder.ApplicationServices.GetRequiredService<ResourceServerOptions>();
            var identityServerOptions = resourceServerOptions.ToIdentityServer();
            var corsOptions = applicationBuilder.ApplicationServices.GetRequiredService<CorsOptions>();

            return applicationBuilder
                .UseCors(o =>
                {
                    o.WithMethods(corsOptions.AllowMethods.ToArray());
                    o.WithHeaders(corsOptions.AllowHeaders.ToArray());
                    o.WithOrigins(corsOptions.AllowOrigins.ToArray());
                    o.WithExposedHeaders(corsOptions.ExposedHeaders.ToArray());
                    o.AllowCredentials();
                })
                .UseIdentityServerAuthentication(identityServerOptions);
        }
    }
}
