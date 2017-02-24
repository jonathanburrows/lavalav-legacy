using Microsoft.Extensions.Logging;

namespace lvl.Web.Logging
{
    /// <summary>
    /// Configurations for logging.
    /// </summary>
    public class LoggingSettings
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
    }
}
