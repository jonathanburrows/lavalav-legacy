using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace lvl.Repositories.Tests.Configuration
{
    /// <summary>
    /// Used by unit tests to read connection strings and configurations from appsettings.json to testing databases.
    /// </summary>
    internal class ConfigurationReader
    {
        public static IntegrationOptions IntegrationSettings { get; }

        static ConfigurationReader()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false);
            var configuration = builder.Build();
            IntegrationSettings = configuration.Get<IntegrationOptions>();
        }
    }
}
