using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace lvl.Web.Logging
{
    /// <summary>
    /// Catches any errors in the pipeline and logs them.
    /// </summary>
    /// <remarks>Was done as MVC does not call Logging on default when an error occurs.</remarks>
    internal class ErrorLoggingMiddleware
    {
        private RequestDelegate Next { get; }
        private ILoggerFactory LoggerFactory { get; }
        private LoggingSettings LoggingSettings { get; }

        public ErrorLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, LoggingSettings loggingSettings)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }
            if (loggingSettings == null)
            {
                throw new ArgumentNullException(nameof(loggingSettings));
            }

            Next = next;
            LoggerFactory = loggerFactory;
            LoggingSettings = loggingSettings;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            try
            {
                await Next.Invoke(httpContext);
            }
            catch (Exception e)
            {
                // If there's a database issue causing the error, we dont want to obscure it, wrap this in an additional try finally
                try
                {
                    var logger = LoggerFactory.CreateLogger<ErrorLoggingMiddleware>();
                    logger.LogError(0, e, e.Message);
                }
                finally
                {
                    throw e;
                }
            }
        }
    }
}
