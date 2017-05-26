using System;

// ReSharper disable once CheckNamespace In compliance with microsoft convention.
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Provides a way to register all services required to act as a resource server.
    /// </summary>
    public static class ResourceServerServiceCollectionExtensions
    {
        /// <summary>
        ///     Registers all services required to act as a resource server.
        /// </summary>
        /// <param name="serviceCollection">The collection of services which will have types registered.</param>
        /// <param name="options">The options required to act as a resource server.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
        /// <returns>The service collection with all services registered.</returns>
        public static IServiceCollection AddResourceServer(this IServiceCollection serviceCollection, ResourceServerOptions options)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (options == null)
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

            return serviceCollection
                .AddSingleton(options)
                .AddAuthentication();
        }
    }
}
