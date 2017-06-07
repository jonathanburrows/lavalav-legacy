using System;
using IdentityServer4;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace In compliance with Microsoft's conventions.
namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    ///     Sets up an Mvc application to act as an open id authentication server.
    /// </summary>
    public static class OidcAuthorizationServerApplicationBuilderExtensions
    {
        /// <summary>
        ///     Sets up an Mvc application to act as an open id authentication server.
        /// </summary>
        /// <param name="applicationBuilder">The application being configured.</param>
        /// <exception cref="ArgumentNullException"><paramref name="applicationBuilder"/> is null.</exception>
        /// <returns>The given application.</returns>
        public static IApplicationBuilder UseOidcAuthorizationServer(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
            {
                throw new ArgumentNullException(nameof(applicationBuilder));
            }

            return applicationBuilder
                .UseCookieAuthentication()
                .UseIdentityServer()
                .UseFacebook();
        }

        private static IApplicationBuilder UseFacebook(this IApplicationBuilder applicationBuilder)
        {
            var options = applicationBuilder.ApplicationServices.GetService<OidcAuthorizationServerOptions>();
            if (options.Facebook == null)
            {
                return applicationBuilder;
            }
            else if (options.Facebook.Id == null)
            {
                throw new InvalidCastException("configuration has no Facebook.Id, please add it to the application secrets.");
            }
            else if (options.Facebook.Secret == null)
            {
                throw new InvalidCastException("configuration has no Facebook.Secret, please add it to the application secrets.");
            }
            else
            {
                return applicationBuilder.UseFacebookAuthentication(new FacebookOptions
                {
                    AuthenticationScheme = "Facebook",
                    DisplayName = "Facebook",
                    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                    AppId = options.Facebook.Id,
                    AppSecret = options.Facebook.Secret
                });
            }
        }
    }
}
