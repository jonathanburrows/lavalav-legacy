using lvl.Web.Cors;
using lvl.Web.Logging;
using Microsoft.Extensions.Configuration;
using System;

namespace lvl.Web
{
    /// <summary>
    /// Wrapper for all settings.
    /// </summary>
    public class WebOptions
    {
        public LoggingOptions Logging { get; set; }
        public CorsOptions Cors { get; set; }
        public string Url { get; set; }

        public WebOptions() { }

        public WebOptions(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.Bind(this);

            Logging = new LoggingOptions();
            configuration.GetSection("logging").Bind(Logging);

            Cors = new CorsOptions();
            configuration.GetSection("cors").Bind(Cors);
        }
    }
}
