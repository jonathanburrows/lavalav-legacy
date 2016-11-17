using Microsoft.Extensions.DependencyInjection;

namespace lvl.Ontology
{
    public static class DomainServiceCollectionExtensions
    {
        public static IServiceCollection AddDomains(this IServiceCollection serviceCollection, string connectionString)
        {
            return serviceCollection;
        }
    }
}
