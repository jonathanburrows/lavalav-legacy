using lvl.Repositories;
using lvl.Repositories.Authorization;
using lvl.Web.Authorization;
using lvl.Web.Logging;
using lvl.Web.OData;
using lvl.Web.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;

// ReSharper disable once CheckNamespace In compliance with Microsoft conventions
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
        /// <param name="webOptions">Settings for how the web service will behave.</param>
        /// <returns>The given service collection with types registered against it.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> is null.</exception>
        /// <exception cref="InvalidOperationException">AddDomains and AddRepositories haven't been called.</exception>
        public static IServiceCollection AddWeb(this IServiceCollection serviceCollection, WebOptions webOptions = null)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            var registeredServices = serviceCollection.Select(s => s.ServiceType).ToList();
            if (!registeredServices.Contains(typeof(NHibernate.Cfg.Configuration)))
            {
                throw new InvalidOperationException($"{nameof(DomainServiceCollectionExtensions.AddDomains)} has not been called");
            }
            if (!registeredServices.Contains(typeof(RepositoryFactory)))
            {
                throw new InvalidOperationException($"{nameof(RepositoryServiceCollectionExtensions.AddRepositories)} has not been called");
            }

            var loggingSettings = webOptions?.Logging ?? new LoggingOptions();
            serviceCollection.AddSingleton(loggingSettings);

            var corsSettings = webOptions?.Cors ?? new CorsOptions();
            serviceCollection.AddSingleton(corsSettings);

            // ReSharper disable once ConvertToLocalFunction No.
            Action<JsonSerializerSettings> configureJson = options =>
            {
                options.MissingMemberHandling = MissingMemberHandling.Error;
                options.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            };

            serviceCollection
                .AddLogging()
                .AddScoped<AggregateRootFilter, AuthorizationFilter>()
                .AddScoped<NHibernate.IInterceptor, AuthorizationInterceptor>()
                .AddScoped<EntityDeserializer>()
                .AddScoped<ILoggerProvider, DatabaseLoggerProvider>()
                .AddScoped<ILoggerFactory, DatabaseLoggerFactory>()
                .AddScoped<ODataConventionTokenizer>()
                .AddScoped<ODataConventionParser>()
                .AddScoped<ODataQueryParser>()
                .AddScoped<Impersonator>()
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
