using lvl.Web.Cors;
using lvl.Web.Logging;
using Microsoft.Extensions.Configuration;

namespace lvl.Web
{
    /// <summary>
    /// Wrapper for all settings.
    /// </summary>
    public class WebSettings
    {
        public LoggingSettings Logging { get; set; }
        public CorsSettings Cors { get; }

        public WebSettings() { }

        public WebSettings(IConfiguration configuration)
        {
            Logging = new LoggingSettings { };
            configuration.GetSection("logging").Bind(Logging);

            Cors = new CorsSettings { };
            configuration.GetSection("cors").Bind(Cors);
        }
    }
}
