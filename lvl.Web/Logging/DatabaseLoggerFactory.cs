using Microsoft.Extensions.Logging;
using System;

namespace lvl.Web.Logging
{
    /// <summary>
    /// Provide a way to construct DatabaseLoggers with types resolved from Service Provider.
    /// </summary>
    internal class DatabaseLoggerFactory : LoggerFactory
    {
        public DatabaseLoggerFactory(ILoggerProvider loggerProvider)
        {
            if (loggerProvider == null)
            {
                throw new ArgumentNullException(nameof(loggerProvider));
            }

            AddProvider(loggerProvider);
        }
    }
}
