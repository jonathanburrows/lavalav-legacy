using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using lvl.Ontology;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides functions for registering repositories in a service provider
    /// </summary>
    public static class DomainServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a database configuration based on the connection string, 
        /// and maps all models inheriting from IEntity
        /// </summary>
        /// <param name="serviceCollection">The services the configuration will be registered to.</param>
        /// <param name="connectionString">The connection string to the database</param>
        /// <returns>The original service collection, with a configuration registered.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/></exception>
        /// <remarks>If the connection string is null, then it will use SQLite</remarks>
        public static IServiceCollection AddDomains(this IServiceCollection serviceCollection, string connectionString = null)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            var callingAssembly = Assembly.GetCallingAssembly();

            var configuration = Fluently
                .Configure()
                .Database(ConstructDatabaseConnection(connectionString))
                .AddReferencedEntities(callingAssembly)
                .BuildConfiguration();

            serviceCollection.AddSingleton(_ => configuration);

            return serviceCollection;
        }

        private static IPersistenceConfigurer ConstructDatabaseConnection(string connectionString)
        {
            var databaseDetector = new DatabaseDetector();
            switch (databaseDetector.GetConnectionStringsVendor(connectionString))
            {
                case DatabaseVendor.SQLite:
                    var interopCopier = new SQLiteInteropCopier();
                    interopCopier.Copy();
                    return SQLiteConfiguration.Standard.InMemory();
                case DatabaseVendor.MsSql:
                    return MsSqlConfiguration.MsSql2012.ConnectionString(connectionString);
                case DatabaseVendor.Oracle:
                    return OracleManagedDataClientConfiguration.Oracle10.ConnectionString(connectionString);
                default:
                    throw new ArgumentException("Database vendor doesnt have nhibernate support");
            }
        }

        private static FluentConfiguration AddReferencedEntities(this FluentConfiguration fluentConfiguration, Assembly callingAssembly)
        {
            var baseType = typeof(IEntity);
            callingAssembly
                .GetReferencedAssemblies()
                .AsParallel()
                .ForAll(a => Assembly.Load(a));

            var assemblies = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .AsParallel()
                .Where(a => !a.IsDynamic)
                .Where(a => a.ExportedTypes.Any(t => baseType.IsAssignableFrom(t)));

            var assemblyMapping = AutoMap
                .Assemblies(assemblies.ToArray())
                .Where(t => baseType.IsAssignableFrom(t))
                .IgnoreBase<IEntity>();

            var conventions = assemblyMapping.Conventions;
            conventions.Add(DefaultCascade.All());
            conventions.Add(LazyLoad.Never());
            conventions.Add(ForeignKey.EndsWith("Id"));

            fluentConfiguration.ExposeConfiguration(assemblyMapping.Configure);

            return fluentConfiguration;
        }
    }
}