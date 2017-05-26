using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using lvl.Ontology;
using System;
using System.Linq;
using System.Reflection;
using lvl.Ontology.Database;
using lvl.Ontology.Conventions;
using NHibernate.Tool.hbm2ddl;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Provides functions for registering repositories in a service provider
    /// </summary>
    public static class DomainServiceCollectionExtensions
    {
        /// <summary>
        ///     Registers a database configuration based on the connection string, 
        ///     and maps all models inheriting from IEntity
        /// </summary>
        /// <param name="serviceCollection">The services the configuration will be registered to.</param>
        /// <param name="domainOptions">The options, including the connection string to the database.</param>
        /// <returns>The original service collection, with a configuration registered.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/></exception>
        /// <remarks>If the connection string is null, then it will use SQLite</remarks>
        public static IServiceCollection AddDomains(this IServiceCollection serviceCollection, DomainOptions domainOptions = null)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }
            domainOptions = domainOptions ?? new DomainOptions();

            var callingAssembly = Assembly.GetCallingAssembly();

            var configuration = Fluently
                .Configure()
                .Database(ConstructDatabaseConnection(domainOptions.ConnectionString))
                .AddReferencedEntities(callingAssembly)
                .BuildConfiguration();

            var foreignIdConvention = new ForeignKeyIdConvention(configuration);
            foreignIdConvention.AddForeignKeyIds();

            SchemaMetadataUpdater.QuoteTableAndColumns(configuration);

            serviceCollection
                .AddSingleton(domainOptions)
                .AddSingleton(_ => configuration);

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
                .Where(AssemblyContainsIEntity);

            var assemblyMapping = AutoMap
                .Assemblies(assemblies.ToArray())
                .Where(t => typeof(Entity).IsAssignableFrom(t))
                .IgnoreBase<Entity>();

            var conventions = assemblyMapping.Conventions;
            conventions.Add(DefaultCascade.All());
            conventions.Add(DefaultLazy.Never());
            conventions.Add(LazyLoad.Never());
            conventions.Add(ForeignKey.EndsWith("Id"));
            conventions.Add<MaxLengthConvention>();
            conventions.Add<RequiredConvention>();
            conventions.Add<TableNamingConvention>();
            conventions.Add<UniqueConvention>();
            conventions.Add<InvalidAggregateRootReferenceConvention>();

            fluentConfiguration.ExposeConfiguration(assemblyMapping.Configure);

            return fluentConfiguration;
        }

        /// <summary>
        ///     Determines if an assembly contains an IEntity.
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
            var ontologyAssembly = typeof(Entity).GetTypeInfo().Assembly;
            if (assembly.GetReferencedAssemblies().All(ra => ra.FullName != ontologyAssembly.FullName))
            {
                return false;
            }

            return assembly.ExportedTypes.Any(t => typeof(Entity).IsAssignableFrom(t));
        }
    }
}