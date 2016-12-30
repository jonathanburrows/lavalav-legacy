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
        private LoggerSettings LoggerSettings { get; }
        private IRepository<LogEntry> LogEntryRepository { get; }

        public DatabaseLogger(string name, LoggerSettings loggerSettings, IRepository<LogEntry> logEntryRepository)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (loggerSettings == null)
            {
                throw new ArgumentNullException(nameof(loggerSettings));
            }
            if (logEntryRepository == null)
            {
                throw new ArgumentNullException(nameof(logEntryRepository));
            }

            Name = name;
            LoggerSettings = loggerSettings;
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
            return LoggerSettings.LogLevel <= logLevel;
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
