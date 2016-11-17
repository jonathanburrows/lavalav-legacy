using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using lvl.Ontology;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainServiceCollectionExtensions
    {
        public static IServiceCollection AddDomains(this IServiceCollection serviceCollection)
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

            serviceCollection.AddSingleton((provider) =>
            {
                var config = Fluently
                    .Configure()
                    .Database(SQLiteConfiguration.Standard.InMemory)
                    .Mappings(mapping =>
                    {
                        var m = assemblies.ToList();
                        var n = domainModels.ToList();
                        domainModels.ForAll(domainModel => mapping.FluentMappings.Add(domainModel));
                    });
                config.BuildSessionFactory();
                return config;
            });

            return serviceCollection;
        }
    }
}
