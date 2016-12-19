using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using lvl.Ontology;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

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
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

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
            var sqlServerPattern = new Regex(@"(i?)(database)|(initial catalog)", RegexOptions.IgnoreCase);
            var oraclePattern = new Regex(@"(i?)(data source)", RegexOptions.IgnoreCase);

            if (string.IsNullOrEmpty(connectionString))
            {
                return SQLiteConfiguration.Standard.InMemory();
            }
            else if (sqlServerPattern.IsMatch(connectionString))
            {
                return MsSqlConfiguration.MsSql2012;
            }
            else if (oraclePattern.IsMatch(connectionString))
            {
                return OracleClientConfiguration.Oracle10;
            }
            else
            {
                throw new ArgumentException($"Connection string does not have an associated database provider");
            }
        }

        private static FluentConfiguration AddReferencedEntities(this FluentConfiguration fluentConfiguration, Assembly callingAssembly)
        {
            var baseType = typeof(IEntity);
            var assemblies = callingAssembly
                .GetReferencedAssemblies()
                .AsParallel()
                .Select(Assembly.Load)
                .Where(a => a.ExportedTypes.Any(t => baseType.IsAssignableFrom(t)))
                .ToList();
            assemblies.Add(callingAssembly);

            var assemblyMapping = AutoMap
                .Assemblies(assemblies.ToArray())
                .Where(t => baseType.IsAssignableFrom(t))
                .IgnoreBase<IEntity>();

            var conventions = assemblyMapping.Conventions;
            conventions.Add(DefaultCascade.All());
            conventions.Add(LazyLoad.Never());

            fluentConfiguration.ExposeConfiguration(assemblyMapping.Configure);

            return fluentConfiguration;
        }
    }
}