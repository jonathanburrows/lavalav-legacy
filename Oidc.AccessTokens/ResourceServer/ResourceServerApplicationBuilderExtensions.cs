using Microsoft.Extensions.DependencyInjection;
using System;

// ReSharper disable once CheckNamespace In compliance with microsoft convention.
namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    ///     Provides a way to add all middleware required to be a resource server.
    /// </summary>
    public static class ResourceServerApplicationBuilderExtensions
    {
        /// <summary>
        ///     Adds all middleware required to be a resource server.
        /// </summary>
        /// <param name="applicationBuilder">The application which will have middleware added to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="applicationBuilder"/> is null.</exception>
        /// <returns>The application with all middleware added.</returns>
        public static IApplicationBuilder UseResourceServer(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
            {
                throw new ArgumentNullException();
            }

            var resourceServerOptions = applicationBuilder.ApplicationServices.GetRequiredService<ResourceServerOptions>();
            var identityServerOptions = resourceServerOptions.ToIdentityServer();

            return applicationBuilder.UseIdentityServerAuthentication(identityServerOptions);
        }
    }
}
