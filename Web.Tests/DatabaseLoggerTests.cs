using lvl.Repositories;
using lvl.Web.Logging;
using lvl.Web.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace lvl.Web.Tests
{
    [Collection(nameof(WebCollection))]
    public class DatabaseLoggerTests
    {
        [Fact]
        public async Task It_will_insert_log_entry_after_logging()
        {
            var services = GetLoggerServicesWithLogLevel(LogLevel.Debug);
            var databaseLogger = services.GetRequiredService<ILogger<DatabaseLoggerTests>>();
            var repository = services.GetRequiredService<IRepository<LogEntry>>();
            var countBefore = (await repository.GetAsync()).Count();

            databaseLogger.LogDebug("hello, world!");

            var countAfter = (await repository.GetAsync()).Count();
            Assert.True(countBefore < countAfter);
        }

        [Fact]
        public async Task It_will_not_insert_log_entry_if_log_level_isnt_enabled()
        {
            var services = GetLoggerServicesWithLogLevel(LogLevel.Error);
            var databaseLogger = services.GetRequiredService<ILogger<DatabaseLoggerTests>>();
            var repository = services.GetRequiredService<IRepository<LogEntry>>();
            var countBefore = (await repository.GetAsync()).Count();

            databaseLogger.LogDebug("hello, world!");

            var countAfter = (await repository.GetAsync()).Count();
            Assert.False(countBefore < countAfter);
        }

        [Theory]
        [InlineData(LogLevel.Trace)]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.Warning)]
        [InlineData(LogLevel.Error)]
        public void It_isnt_enabled_when_log_level_is_lower_than_configured(LogLevel logLevel)
        {
            var services = GetLoggerServicesWithLogLevel(logLevel + 1);
            var databaseLogger = services.GetRequiredService<ILogger<DatabaseLoggerTests>>();

            var isEnabled = databaseLogger.IsEnabled(logLevel);

            Assert.False(isEnabled);
        }

        [Theory]
        [InlineData(LogLevel.Trace)]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.Warning)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Critical)]
        public void It_is_enabled_when_log_level_is_equal_to_what_is_configured(LogLevel logLevel)
        {
            var services = GetLoggerServicesWithLogLevel(logLevel);
            var databaseLogger = services.GetRequiredService<ILogger<DatabaseLoggerTests>>();

            var isEnabled = databaseLogger.IsEnabled(logLevel);

            Assert.True(isEnabled);
        }

        [Theory]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.Warning)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Critical)]
        public void It_is_enabled_when_log_level_is_higher_than_what_is_configured(LogLevel logLevel)
        {
            var services = GetLoggerServicesWithLogLevel(logLevel - 1);
            var databaseLogger = services.GetRequiredService<ILogger<DatabaseLoggerTests>>();

            var isEnabled = databaseLogger.IsEnabled(logLevel);

            Assert.True(isEnabled);
        }

        private IServiceProvider GetLoggerServicesWithLogLevel(LogLevel logLevel)
        {
            var webSettings = new WebOptions
            {
                Logging = new LoggingOptions
                {
                    LogLevel = logLevel
                }
            };
            return new ServiceCollection()
                .AddDomains()
                .AddDatabaseGeneration()
                .AddRepositories()
                .AddWeb(webSettings)
                .BuildServiceProvider();
        }
    }
}
