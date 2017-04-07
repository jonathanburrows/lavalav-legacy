using lvl.Oidc.Seeder;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AddOidcSeederServiceCollectionExtensions
    {
        public static IServiceCollection AddOidcSeeder(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection
                .AddScoped<ArgumentParser>()
                .AddScoped<ManditoryDataSeeder>()
                .AddScoped<TestDataSeeder>();
        }
    }
}
