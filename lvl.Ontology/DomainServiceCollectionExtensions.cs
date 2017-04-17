using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using lvl.Ontology;
using lvl.Ontology.Naming;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace
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

            var namingApplier = new NamingApplier();
            configuration.ClassMappings.ToList().ForEach(namingApplier.Apply);
            
            SchemaMetadataUpdater.QuoteTableAndColumns(configuration);

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
            callingAssembly
                .GetReferencedAssemblies()
                .AsParallel()
                .ForAll(a => Assembly.Load(a));

            var assemblies = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .AsParallel()
                .Where(AssemblyContainsIEntity)
                .ToArray();

            var assemblyMapping = AutoMap
                .Assemblies(assemblies)
                .Where(t => typeof(IEntity).IsAssignableFrom(t))
                .IgnoreBase<IEntity>();

            foreach (var assembly in assemblies)
            {
                assemblyMapping.AddMappingsFromAssembly(assembly);
                assemblyMapping.Conventions.AddAssembly(assembly);
                assemblyMapping.UseOverridesFromAssembly(assembly);
            }

            var conventions = assemblyMapping.Conventions;

            conventions.Add(DefaultCascade.All());
            conventions.Add(LazyLoad.Never());
            conventions.Add(ForeignKey.EndsWith("Id"));
            conventions.Add<StringLengthConvention>();

            fluentConfiguration.ExposeConfiguration(assemblyMapping.Configure);

            return fluentConfiguration;
        }

        /// <summary>
        /// Determines if an assembly contains an IEntity.
        /// </summary>
        /// <param name="assembly">The assembly to be checked.</param>
        /// <returns>true if the assembly contains an ientity, false otherwise.</returns>
        /// <remarks>Much of this was done to prevent 3rd party libraries from crashing the app.</remarks>
        private static bool AssemblyContainsIEntity(Assembly assembly)
        {
            // this was done to prevent nhibernate from crashing the app.
            if (assembly.IsDynamic)
            {
                return false;
            }

            // this was done to prevent a preview release version of xunit from crashing
            var ontologyAssembly = typeof(IEntity).GetTypeInfo().Assembly;
            if (!assembly.GetReferencedAssemblies().Any(ra => ra.FullName == ontologyAssembly.FullName))
            {
                return false;
            }

            return assembly.ExportedTypes.Any(t => typeof(IEntity).IsAssignableFrom(t));
        }
    }
}