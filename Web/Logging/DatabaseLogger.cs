using System;
using Microsoft.Extensions.Logging;
using lvl.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace lvl.Web.Logging
{
    /// <summary>
    /// Provides a way to log information into a database using nhibernate.
    /// </summary>
    internal class DatabaseLogger : ILogger
    {
        private LoggingOptions LoggingSettings { get; }
        private IRepository<LogEntry> LogEntryRepository { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }

        public DatabaseLogger(string name, LoggingOptions loggingSettings, IRepository<LogEntry> logEntryRepository, IHttpContextAccessor httpContextAccessor)
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
            if (httpContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }

            LoggingSettings = loggingSettings;
            LogEntryRepository = logEntryRepository;
            HttpContextAccessor = httpContextAccessor;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            return LogScope.Push(state);
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
            var context = HttpContextAccessor?.HttpContext;
            var request = context?.Request;
            var userName = context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var url = request?.Path != PathString.Empty ? request?.GetDisplayUrl() : null;

            var logEntry = new LogEntry
            {
                MachineName = Environment.MachineName,
                Logged = DateTime.Now,
                LogLevel = logLevel.ToString(),
                Message = message,
                StackTrace = exception?.StackTrace,
                Exception = exception?.GetType().Name,
                Https = request?.IsHttps,
                Url = url,
                UserName = userName
            };

            LogEntryRepository.CreateAsync(logEntry).Wait();
        }
    }
}
