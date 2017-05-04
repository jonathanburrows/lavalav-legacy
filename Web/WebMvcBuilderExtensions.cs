using lvl.Web.Cors;
using lvl.Web.Logging;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

// ReSharper disable once CheckNamespace In compliance with Microsoft conventions.
namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Provide a way to build the middleware pipeline for web requests.
    /// </summary>
    public static class WebMvcBuilderExtensions
    {
        /// <summary>
        /// Adds the middleware used in the API pipeline, and sets up routing.
        /// </summary>
        /// <param name="applicationBuilder">The builder for the web application.</param>
        /// <param name="configureRoutes">A function which modifies the registered routes.</param>
        /// <returns>The application builder with the middleware registered against it.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="applicationBuilder"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="configureRoutes"/> is null.</exception>
        /// <exception cref="InvalidOperationException">AddWeb has not been called.</exception>
        public static IApplicationBuilder UseWeb(this IApplicationBuilder applicationBuilder, Action<IRouteBuilder> configureRoutes)
        {
            if (applicationBuilder == null)
            {
                throw new ArgumentNullException(nameof(applicationBuilder));
            }
            if (configureRoutes == null)
            {
                throw new ArgumentNullException(nameof(configureRoutes));
            }

            var corsSettings = applicationBuilder.ApplicationServices.GetService<CorsSettings>();
            if (corsSettings == null)
            {
                throw new InvalidOperationException($"{nameof(WebServiceCollectionExtensions.AddWeb)} has not been called.");
            }

            Action<CorsPolicyBuilder> corsConfiguration = corsBuilder =>
            {
                corsBuilder.WithMethods(corsSettings.AllowMethods.ToArray());
                corsBuilder.WithHeaders(corsSettings.AllowHeaders.ToArray());
                corsBuilder.WithOrigins(corsSettings.AllowOrigins.ToArray());
                corsBuilder.WithExposedHeaders(corsSettings.ExposedHeaders.ToArray());
            };

            // sets up the default routs before calling the given configuration method.
            Action<IRouteBuilder> defaultRouteBuilder = routes =>
            {
                routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
                configureRoutes.Invoke(routes);
            };


            return applicationBuilder
                .UseMiddleware<ErrorLoggingMiddleware>()
                .UseCors(corsConfiguration)
                .UseMvc(defaultRouteBuilder);
        }
        
        /// <summary>
        /// Adds the middleware used in the API pipeline, and sets up routing.
        /// </summary>
        /// <param name="applicationBuilder">The builder for the web application.</param>
        /// <returns>The application builder with the middleware registered against it.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="applicationBuilder"/> is null.</exception>
        public static IApplicationBuilder UseWeb(this IApplicationBuilder applicationBuilder) => UseWeb(applicationBuilder, _ => { });
    }
}
