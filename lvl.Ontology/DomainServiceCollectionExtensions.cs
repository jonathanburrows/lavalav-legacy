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
    public static class DomainServiceCollectionExtensions
    {
        public static IServiceCollection AddDomains(this IServiceCollection serviceCollection, string connectionString = null)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            var config = Fluently
                .Configure()
                .Database(ConstructDatabaseConnection(connectionString))
                .AddReferencedEntities(callingAssembly);

            serviceCollection.AddSingleton(provider => config.BuildConfiguration());

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
