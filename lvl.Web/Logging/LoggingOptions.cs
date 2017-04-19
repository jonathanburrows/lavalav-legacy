using Microsoft.Extensions.Logging;

namespace lvl.Web.Logging
{
    /// <summary>
    /// Configurations for logging.
    /// </summary>
    public class LoggingOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
    }
}
