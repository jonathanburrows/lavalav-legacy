using lvl.DatabaseGenerator;
using System;

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
        public static IServiceCollection AddDatabaseGeneration(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.AddScoped<DatabaseCreator>();
            serviceCollection.AddScoped<DatabaseMigrator>();
            serviceCollection.AddScoped<ScriptRunner>();

            return serviceCollection;
        }
    }
}
