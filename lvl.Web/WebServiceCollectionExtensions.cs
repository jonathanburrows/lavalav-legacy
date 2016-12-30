using lvl.Repositories;
using lvl.Web;
using lvl.Web.Logging;
using lvl.Web.Serialization;
using Microsoft.Extensions.Logging;
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
        /// <param name="webSettings">Settings for how the web service will behave.</param>
        /// <returns>The given service collection with types registered against it.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> is null.</exception>
        /// <exception cref="InvalidOperationException">AddDomains and AddRepositories haven't been called.</exception>
        public static IServiceCollection AddWeb(this IServiceCollection serviceCollection, WebSettings webSettings)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }
            if (webSettings == null)
            {
                throw new ArgumentNullException(nameof(webSettings));
            }
            if (webSettings.Logging == null)
            {
                throw new ArgumentNullException(nameof(webSettings.Logging));
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

            Action<JsonSerializerSettings> configureJson = options =>
            {
                options.MissingMemberHandling = MissingMemberHandling.Error;
            };

            serviceCollection
                .AddLogging()
                .AddScoped<EntityDeserializer>()
                .AddSingleton(webSettings.Logging)
                .AddScoped<ILoggerProvider, DatabaseLoggerProvider>()
                .AddScoped<ILoggerFactory, DatabaseLoggerFactory>()
                .AddOptions()
                .Configure(configureJson)
                .AddCors()
                .AddMvcCore()
                .AddJsonFormatters()
                .AddApplicationPart(typeof(WebServiceCollectionExtensions).Assembly)
                .AddControllersAsServices();

            return serviceCollection;
        }

        /// <summary>
        /// Registers all types required by web middleware.
        /// </summary>
        /// <param name="serviceCollection">The service collection which will have types registered against it.</param>
        /// <returns>The given service collection with types registered against it.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> is null.</exception>
        /// <exception cref="InvalidOperationException">AddDomains and AddRepositories haven't been called.</exception>
        public static IServiceCollection AddWeb(this IServiceCollection serviceCollection)
        {
            var webSettings = new WebSettings
            {
                Logging = new LoggingSettings { }
            };
            return AddWeb(serviceCollection, webSettings);
        }
    }
}
