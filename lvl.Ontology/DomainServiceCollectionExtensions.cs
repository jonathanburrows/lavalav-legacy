using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
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
            var assemblies = Assembly
                .GetCallingAssembly()
                .GetReferencedAssemblies()
                .AsParallel()
                .Select(Assembly.Load);

            var baseModel = typeof(IEntity);
            var domainModels = assemblies
                .SelectMany(a => a.ExportedTypes)
                .Where(t => baseModel.IsAssignableFrom(t))
                .Where(entityType => entityType.GetConstructor(Type.EmptyTypes) != null);

            serviceCollection.AddSingleton(provider =>
            {
                var config = Fluently
                    .Configure()
                    .Database(SQLiteConfiguration.Standard.InMemory)
                    .Mappings(mapping => domainModels.ForAll(domainModel => mapping.FluentMappings.Add(domainModel)));
                config.BuildSessionFactory();
                return config;
            });

            return serviceCollection;
        }

        private static IPersistenceConfigurer ConstructDatabaseConnection(string connectionString)
        {
            var sqlServerPattern = new Regex(@"");
            var oraclePattern = new Regex(@"");

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
    }
}
