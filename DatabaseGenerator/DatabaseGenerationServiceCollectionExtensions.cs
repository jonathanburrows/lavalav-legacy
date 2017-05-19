using lvl.DatabaseGenerator;
using System;
using System.Linq;

// ReSharper disable once CheckNamespace In compliance with Microsoft's conventions.
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Registers all services specific to generate databases.
    /// </summary>
    public static class DatabaseGenerationServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all services specific to generate databases.
        /// </summary>
        /// <param name="serviceCollection">The provider which will have the services registered.</param>
        /// <param name="options">Options used when generating a database.</param>
        /// <returns>The given provider, with all services registered.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> is null.</exception>
        /// <exception cref="InvalidOperationException">AddDomains has not been called.</exception>
        public static IServiceCollection AddDatabaseGeneration(this IServiceCollection serviceCollection, DatabaseGenerationOptions options = null)
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

            options = options ?? new DatabaseGenerationOptions();

            serviceCollection.AddSingleton(options);
            serviceCollection.AddScoped<DatabaseCreator>();
            serviceCollection.AddScoped<DatabaseMigrator>();
            serviceCollection.AddScoped<ScriptRunner>();
            serviceCollection.AddScoped<DatabaseGenerationRunner>();

            return serviceCollection;
        }
    }                                                                                                    
}
