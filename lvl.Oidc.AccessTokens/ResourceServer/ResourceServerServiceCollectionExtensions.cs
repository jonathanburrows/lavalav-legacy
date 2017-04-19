using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ResourceServerServiceCollectionExtensions
    {
        public static IServiceCollection AddResourceServer(this IServiceCollection serviceCollection)
        {
            if(serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection.AddAuthentication();
        }
    }
}
