using System;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using lvl.Repositories;

namespace lvl.Web.Logging
{
    /// <summary>
    /// Constructs database loggers with types resolved from the service provider.
    /// </summary>
    internal class DatabaseLoggerProvider : ILoggerProvider
    {
        private LoggerSettings Settings { get; }
        private ConcurrentDictionary<string, DatabaseLogger> Loggers { get; }
        private IRepository<LogEntry> LogEntryRepository { get; }

        public DatabaseLoggerProvider(LoggerSettings settings, IRepository<LogEntry> logEntryRepository)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (logEntryRepository == null)
            {
                throw new ArgumentNullException(nameof(logEntryRepository));
            }

            Settings = settings;
            Loggers = new ConcurrentDictionary<string, DatabaseLogger>();
            LogEntryRepository = logEntryRepository;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return Loggers.GetOrAdd(categoryName, name => new DatabaseLogger(name, Settings, LogEntryRepository));
        }

        public void Dispose() { }
    }
}
