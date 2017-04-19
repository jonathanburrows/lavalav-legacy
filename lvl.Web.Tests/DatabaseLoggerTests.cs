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
    [Collection(WebCollection.Name)]
    public class DatabaseLoggerTests
    {
        [Fact]
        public async Task AfterLogging_LogEntryIsInserted()
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
        public async Task WhenLogging_AndLogLevelIsntEnabled_LogEntryIsNotInserted()
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
        public void IsEnabled_WhenLowerThanEnabled_ReturnsFalse(LogLevel logLevel)
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
        public void IsEnabled_WhenEqualToEnabled_ReturnsTrue(LogLevel logLevel)
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
        public void IsEnabled_WhenHigherThanEnabled_ReturnsTrue(LogLevel logLevel)
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
