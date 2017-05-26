using System;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using lvl.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace lvl.Web.Logging
{
    /// <summary>
    ///     Constructs database loggers with types resolved from the service provider.
    /// </summary>
    internal class DatabaseLoggerProvider : ILoggerProvider
    {
        private LoggingOptions Settings { get; }
        private ConcurrentDictionary<string, DatabaseLogger> Loggers { get; }
        private IRepository<LogEntry> LogEntryRepository { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }

        public DatabaseLoggerProvider(LoggingOptions settings, IRepository<LogEntry> logEntryRepository, IHttpContextAccessor httpContextAccessor)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Loggers = new ConcurrentDictionary<string, DatabaseLogger>();
            LogEntryRepository = logEntryRepository ?? throw new ArgumentNullException(nameof(logEntryRepository));
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public ILogger CreateLogger(string categoryName)
        {
            return Loggers.GetOrAdd(categoryName, name => new DatabaseLogger(name, Settings, LogEntryRepository, HttpContextAccessor));
        }

        public void Dispose() { }
    }
}
