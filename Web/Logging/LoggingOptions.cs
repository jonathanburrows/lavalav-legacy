using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace lvl.Web.Logging
{
    /// <summary>
    /// Configurations for logging.
    /// </summary>
    public class LoggingOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        public LoggingOptions() { }

        public LoggingOptions(IConfiguration configuration)
        {
            if(configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.GetSection("logging").Bind(this);
        }
    }
}
