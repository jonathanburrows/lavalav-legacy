using System;

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
                .UseIdentityServer();
        }
    }
}
