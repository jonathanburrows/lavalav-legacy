using IdentityModel.Client;
using lvl.Oidc.AccessTokens.ResourceServer;
using Microsoft.Extensions.DependencyInjection;
using System;

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

            return applicationBuilder
                .UseCors(o =>
                {
                    o.AllowAnyOrigin();
                    o.AllowAnyMethod();
                    o.AllowAnyHeader();
                    o.AllowCredentials();
                })
                .UseIdentityServerAuthentication(identityServerOptions);
        }
    }
}
