using lvl.Oidc.AuthorizationServer.ViewModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace In compliance with Microsoft's conventions.
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Contains options used by an authorization server.
    /// </summary>
    public class OidcAuthorizationServerOptions
    {
        /// <summary>
        ///     If true, will populate data used in development and unit tests.
        /// </summary>
        public bool SeedTestData { get; set; }

        /// <summary>
        ///     If true, will populate data required by the application.
        /// </summary>
        /// <remarks>
        ///     Defaults to true.
        /// </remarks>
        public bool SeedManditoryData { get; set; } = true;

        /// <summary>
        ///     List of active directory servers which can be authenticated against.
        /// </summary>
        public IList<ExternalProvider> WindowsProviders { get; set; } = new List<ExternalProvider>();

        /// <summary>
        ///     Information needed to use facebook as an Independent Service Provider.
        /// </summary>
        /// <remarks>
        ///     Load this from application secrets, DONT use the config file.
        /// </remarks>
        public ExternalApplicationInformation Facebook { get; set; }

        public OidcAuthorizationServerOptions() { }

        public OidcAuthorizationServerOptions(IConfiguration configuration)
        {
            configuration.GetSection("oidc:authorization-server").Bind(this);
        }
    }
}
