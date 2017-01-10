using lvl.Repositories;
using lvl.Web;
using lvl.Web.Cors;
using lvl.Web.Logging;
using lvl.Web.OData;
using lvl.Web.Serialization;
using Microsoft.AspNetCore.Http;
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
        public static IServiceCollection AddWeb(this IServiceCollection serviceCollection, WebSettings webSettings = null)
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

            var loggingSettings = webSettings?.Logging ?? new LoggingSettings { };
            serviceCollection.AddSingleton(loggingSettings);

            var corsSettings = webSettings?.Cors ?? new CorsSettings { };
            serviceCollection.AddSingleton(corsSettings);

            Action<JsonSerializerSettings> configureJson = options =>
            {
                options.MissingMemberHandling = MissingMemberHandling.Error;
                options.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            };

            serviceCollection
                .AddLogging()
                .AddScoped<EntityDeserializer>()
                .AddScoped<ILoggerProvider, DatabaseLoggerProvider>()
                .AddScoped<ILoggerFactory, DatabaseLoggerFactory>()
                .AddScoped<ODataConventionTokenizer>()
                .AddScoped<ODataConventionParser>()
                .AddScoped<ODataQueryParser>()
                .AddScoped<IHttpContextAccessor, HttpContextAccessor>()
                .AddOptions()
                .AddCors()
                .Configure(configureJson)
                .AddMvcCore()
                .AddJsonFormatters(configureJson)
                .AddApplicationPart(typeof(WebServiceCollectionExtensions).Assembly)
                .AddControllersAsServices();

            return serviceCollection;
        }
    }
}
