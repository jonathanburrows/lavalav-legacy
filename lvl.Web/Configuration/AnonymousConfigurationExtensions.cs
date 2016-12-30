using lvl.Web.Configuration;
using System;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Used to initialize configurations without the file system.
    /// </summary>
    public static class AnonymousConfigurationExtensions
    {
        /// <summary>
        /// Adds a configuration based on the given anoynmous object. For each property of the variable, 
        /// a configuration property will be assigned.
        /// </summary>
        /// <typeparam name="TAnonymous">The type of the anonymous object.</typeparam>
        /// <param name="configurationBuilder">The onfiguration which will be populated by the variables properties.</param>
        /// <param name="anonymous">The object whos properties will be applied to the configuration.</param>
        /// <returns>The configuration with all properties of the anoynmous object assigned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="configurationBuilder"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="anonymous"/> is null.</exception>
        /// <remarks>This was done to keep consistent with Microsoft's implementation.</remarks>
        /// <example>
        /// 
        /// configurationBuilder.AddAnyonmous(new 
        /// {
        ///     Connections = new Dictionary*string, string*
        ///     {
        ///         ["ms-sql"] = "data source=.;initial_catalog=local";
        ///     }
        /// });
        /// </example>
        public static IConfigurationBuilder AddAnonymous(this IConfigurationBuilder configurationBuilder, object anonymous)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }
            if (anonymous == null)
            {
                throw new ArgumentNullException(nameof(anonymous));
            }

            configurationBuilder.Add(new AnonymousConfigurationSource(anonymous));

            return configurationBuilder;
        }
    }
}
