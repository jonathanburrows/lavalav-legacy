using lvl.Repositories;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides a method to register all classes used to resolve repositories from a service provider
    /// </summary>
    public static class RepositoryServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all classes used to resolve repositories.
        /// </summary>
        /// <param name="serviceCollection">The service collection which will have the types registered against it</param>
        /// <returns>The given service collection, with the registered types.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> cannot be null.</exception>
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection) {
            if (serviceCollection == null) throw new ArgumentNullException();

            serviceCollection.AddScoped<TypeResolver>();

            return serviceCollection;
        }
    }
}
