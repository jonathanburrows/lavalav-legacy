using lvl.Oidc.AccessTokens.ResourceServer;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ResourceServerServiceCollectionExtensions
    {
        public static IServiceCollection AddResourceServer(this IServiceCollection serviceCollection, ResourceServerOptions options)
        {
            if(serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.Authority == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.Authority)} is null.");
            }
            if (options.ApiName == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.ApiName)} is null.");
            }
            if (options.ApiSecret == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.ApiSecret)} is null.");
            }

            return serviceCollection
                .AddSingleton(options)
                .AddAuthentication();
        }
    }
}
