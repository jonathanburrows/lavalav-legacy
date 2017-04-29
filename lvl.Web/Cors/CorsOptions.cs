using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lvl.Web.Cors
{
    public class CorsOptions
    {
        public IEnumerable<string> AllowHeaders { get; set; } = new[] { "authorization" };
        public IEnumerable<string> AllowMethods { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> AllowOrigins { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> ExposedHeaders { get; set; } = Enumerable.Empty<string>();

        public CorsOptions() { }

        public CorsOptions(IConfiguration configuration)
        {
            if(configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            AllowHeaders = configuration
                .GetSection($"cors:{nameof(AllowHeaders)}")
                .GetChildren()
                .Select(c => c.Value)
                .Union(new[] { "authorization" });

            AllowMethods = configuration.GetSection($"cors:{nameof(AllowMethods)}").GetChildren().Select(c => c.Value);
            AllowOrigins = configuration.GetSection($"cors:{nameof(AllowOrigins)}").GetChildren().Select(c => c.Value);
            ExposedHeaders = configuration.GetSection($"cors:{nameof(ExposedHeaders)}").GetChildren().Select(c => c.Value);
        }
    }
}
