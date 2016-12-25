using lvl.Ontology;
using lvl.Repositories;
using NHibernate;
using System;
using System.Linq;

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
        /// <exception cref="InvalidOperationException"><paramref name="serviceCollection"/> hasnt had AddDomains called.</exception>
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null) throw new ArgumentNullException();

            var services = serviceCollection.BuildServiceProvider();
            var configuration = services.GetRequiredService<NHibernate.Cfg.Configuration>();
            if (configuration == null)
            {
                throw new InvalidOperationException($"{nameof(DomainServiceCollectionExtensions.AddDomains)} has not been called.");
            }

            var databaseDetector = new DatabaseDetector();
            var databaseVendor = databaseDetector.GetConfigurationsVendor(configuration);
            var sessionManager = databaseVendor == DatabaseVendor.SQLite ? typeof(SQLitePersistentSessionManager) : typeof(SessionManager);
            
            serviceCollection
                .AddScoped<TypeResolver>()
                .AddScoped<RepositoryFactory>()
                .AddScoped(typeof(IRepository<>), typeof(Repository<>))
                .AddScoped(typeof(SessionManager), sessionManager)
                .AddScoped<ISessionFactory>(provider =>
                {
                    var config = provider.GetRequiredService<NHibernate.Cfg.Configuration>();
                    return config.BuildSessionFactory();
                });

            return serviceCollection;
        }
    }
}
