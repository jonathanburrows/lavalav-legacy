﻿using IdentityServer4;
using lvl.Oidc.AuthorizationServer;
using lvl.Oidc.AuthorizationServer.Middleware;
using lvl.Oidc.AuthorizationServer.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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

            var options = applicationBuilder.ApplicationServices.GetRequiredService<OidcAuthorizationServerOptions>();

            return applicationBuilder
                .UseCors(o => o.AllowAnyOrigin())
                .UseCookieAuthentication()
                .UseIdentityServer()
                .UseFacebook(options.Facebook)
                .UseMiddleware<EmbeddedFileMiddleware>();
        }

        private static IApplicationBuilder UseFacebook(this IApplicationBuilder applicationBuilder, ExternalApplicationInformation facebookInformation)
        {
            if (facebookInformation == null)
            {
                return applicationBuilder;
            }
            else if (facebookInformation.Id == null)
            {
                throw new InvalidOperationException($"configuration has no Facebook.Id value.");
            }
            else if (facebookInformation.Secret == null)
            {
                throw new InvalidOperationException($"configuration has no Facebook.Secret value.");
            }
            else
            {
                return applicationBuilder.UseFacebookAuthentication(new FacebookOptions
                {
                    AuthenticationScheme = "Facebook",
                    DisplayName = "Facebook",
                    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                    AppId = facebookInformation.Id,
                    AppSecret = facebookInformation.Secret
                });
            }
        }
    }
}
