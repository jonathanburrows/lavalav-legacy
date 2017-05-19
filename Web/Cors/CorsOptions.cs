using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace In compliance with Microsoft's conventions.
namespace Microsoft.Extensions.DependencyInjection
{
    public class CorsOptions
    {
        public IEnumerable<string> AllowHeaders { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> AllowMethods { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> AllowOrigins { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> ExposedHeaders { get; set; } = Enumerable.Empty<string>();

        public CorsOptions() { }

        public CorsOptions(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            AllowHeaders = GetCollection(configuration, nameof(AllowHeaders));
            AllowMethods = GetCollection(configuration, nameof(AllowMethods));
            AllowOrigins = GetCollection(configuration, nameof(AllowOrigins));
            ExposedHeaders = GetCollection(configuration, nameof(ExposedHeaders));
        }

        private IEnumerable<string> GetCollection(IConfiguration configuration, string key) => configuration.GetSection($"cors:{key}").GetChildren().Select(c => c.Value);
    }
}
