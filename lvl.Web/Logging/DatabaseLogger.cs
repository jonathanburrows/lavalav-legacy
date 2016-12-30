using System;
using Microsoft.Extensions.Logging;
using lvl.Repositories;

namespace lvl.Web.Logging
{
    /// <summary>
    /// Provides a way to log information into a database using nhibernate.
    /// </summary>
    internal class DatabaseLogger : ILogger
    {
        private string Name { get; }
        private LoggingSettings LoggingSettings { get; }
        private IRepository<LogEntry> LogEntryRepository { get; }

        public DatabaseLogger(string name, LoggingSettings loggingSettings, IRepository<LogEntry> logEntryRepository)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (loggingSettings == null)
            {
                throw new ArgumentNullException(nameof(loggingSettings));
            }
            if (logEntryRepository == null)
            {
                throw new ArgumentNullException(nameof(logEntryRepository));
            }

            Name = name;
            LoggingSettings = loggingSettings;
            LogEntryRepository = logEntryRepository;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            return LogScope.Push(Name, state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return LoggingSettings.LogLevel <= logLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }


            var message = formatter(state, exception);

            var logEntry = new LogEntry
            {
                MachineName = Environment.MachineName,
                Logged = DateTime.Now,
                LogLevel = logLevel.ToString(),
                Message = message
            };

            LogEntryRepository.CreateAsync(logEntry).Wait();
        }
    }
}
