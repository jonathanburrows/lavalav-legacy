using lvl.Repositories;
using lvl.Web.Serialization;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides a method for registering all types required by web middleware.
    /// </summary>
    public static class WebServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all types required by web middleware.
        /// </summary>
        /// <param name="serviceCollection">The service collection which will have types registered against it.</param>
        /// <returns>The given service collection with types registered against it.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> is null.</exception>
        /// <exception cref="InvalidOperationException">AddDomains and AddRepositories haven't been called.</exception>
        public static IServiceCollection AddWeb(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            var registeredServices = serviceCollection.Select(s => s.ServiceType);
            if (!registeredServices.Contains(typeof(NHibernate.Cfg.Configuration)))
            {
                throw new InvalidOperationException($"{nameof(DomainServiceCollectionExtensions.AddDomains)} has not been called");
            }
            if (!registeredServices.Contains(typeof(RepositoryFactory)))
            {
                throw new InvalidOperationException($"{nameof(RepositoryServiceCollectionExtensions.AddRepositories)} has not been called");
            }

            serviceCollection
                .AddLogging()
                .AddScoped<EntityDeserializer>()
                .AddOptions()
                .ConfigureJsonOptions()
                .AddMvcCore()
                .AddJsonFormatters()
                .AddApplicationPart(typeof(WebServiceCollectionExtensions).Assembly)
                .AddControllersAsServices();

            return serviceCollection;
        }

        public static IServiceCollection ConfigureJsonOptions(this IServiceCollection serviceCollection)
        {
            serviceCollection.Configure<JsonSerializerSettings>(options =>
            {
                options.MissingMemberHandling = MissingMemberHandling.Error;
            });

            return serviceCollection;
        }
    }
}
