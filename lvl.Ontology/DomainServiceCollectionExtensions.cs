using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using lvl.Ontology;
using System;
using System.IO;
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
            var databaseDetector = new DatabaseDetector();
            switch (databaseDetector.GetConnectionStringsVendor(connectionString)) {
                case DatabaseVendor.SQLite:
                    WriteSQLiteInterop();
                    return SQLiteConfiguration.Standard.InMemory();
                case DatabaseVendor.MsSql:
                    return MsSqlConfiguration.MsSql2012.ConnectionString(connectionString);
                case DatabaseVendor.Oracle:
                    return OracleClientConfiguration.Oracle10.ConnectionString(connectionString);
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
                .Where(a => a.ExportedTypes.Any(t => baseType.IsAssignableFrom(t))).ToList();

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

        private static void WriteSQLiteInterop()
        {
            var currentAssembly = typeof(DomainServiceCollectionExtensions).Assembly;

            var x86Info = currentAssembly.GetManifestResourceStream("lvl.Ontology.x86.SQLite.Interop.dll");
            var x86File = new FileInfo("x86/SQLite.Interop.dll");
            x86File.Directory.Create();
            if (!x86File.Exists)
            {
                using (var x86Stream = x86File.Create())
                {
                    x86Info.Seek(0, SeekOrigin.Begin);
                    x86Info.CopyTo(x86Stream);
                }
            }

            var x64Info = currentAssembly.GetManifestResourceStream("lvl.Ontology.x64.SQLite.Interop.dll");
            var x64File = new FileInfo("x64/SQLite.Interop.dll");
            if (!x64File.Exists)
            {
                x64File.Directory.Create();
                using (var x64Stream = x64File.Create())
                {
                    x64Info.Seek(0, SeekOrigin.Begin);
                    x64Info.CopyTo(x64Stream);
                }
            }
        }
    }
}