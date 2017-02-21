using lvl.DatabaseGenerator;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Registers all services specific to generate databases.
    /// </summary>
    public static class DatabaseGeneratorServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all services specific to generate databases.
        /// </summary>
        /// <param name="serviceCollection">The provider which will have the services registered.</param>
        /// <returns>The given provider, with all services registered.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> is null.</exception>
        /// <exception cref="InvalidOperationException">AddDomains has not been called.</exception>
        public static IServiceCollection AddDatabaseGeneration(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            var registeredTypes = serviceCollection.Select(s => s.ServiceType);
            if (!registeredTypes.Contains(typeof(NHibernate.Cfg.Configuration)))
            {
                throw new InvalidOperationException($"{nameof(DomainServiceCollectionExtensions.AddDomains)} has not been called");
            }

            serviceCollection.AddScoped<DatabaseCreator>();
            serviceCollection.AddScoped<DatabaseMigrator>();
            serviceCollection.AddScoped<ScriptRunner>();

            return serviceCollection;
        }
    }
}
