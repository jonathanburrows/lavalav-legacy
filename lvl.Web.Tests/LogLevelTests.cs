using lvl.Repositories;
using lvl.TestDomain;
using lvl.TestWebSite.Tests.Fixtures;
using lvl.Web.Logging;
using lvl.Web.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace lvl.Web.Tests
{
    [Collection(WebCollection.Name)]
    public class LogLevelTests
    {
        private HttpClient Client { get; }
        private IServiceProvider Services { get; }

        public LogLevelTests(WebHostFixture<TestWebSite.Startup> webHostFixture)
        {
            Client = webHostFixture.Client;
            Services = webHostFixture.Services;
        }

        [Fact]
        public async Task WhenDoingGetRequest_AndLogLevelIsDebug_LogEntryIsAdded()
        {
            var loggingSettings = Services.GetRequiredService<LoggingSettings>();
            loggingSettings.LogLevel = LogLevel.Debug;
            var repository = Services.GetRequiredService<IRepository<LogEntry>>();
            var url = $"/api/{nameof(Moon)}";
            var countBefore = (await repository.GetAsync()).Where(log => log.LogLevel == LogLevel.Debug.ToString()).Count();

            await Client.GetAsync(url);

            var countAfter = (await repository.GetAsync()).Where(log => log.LogLevel == LogLevel.Debug.ToString()).Count();
            Assert.True(countBefore < countAfter);
        }

        [Fact]
        public async Task WhenDoingGetRequest_AndLogLevelIsHigherThanDebug_LogEntryIsNotAdded()
        {
            var loggingSettings = Services.GetRequiredService<LoggingSettings>();
            loggingSettings.LogLevel = LogLevel.Information;
            var repository = Services.GetRequiredService<IRepository<LogEntry>>();
            var url = $"/api/{nameof(Moon)}";
            var countBefore = (await repository.GetAsync()).Where(log => log.LogLevel == LogLevel.Debug.ToString()).Count();

            await Client.GetAsync(url);

            var countAfter = (await repository.GetAsync()).Where(log => log.LogLevel == LogLevel.Debug.ToString()).Count();
            Assert.Equal(countBefore, countAfter);
        }

        [Fact]
        public async Task WhenDoingGetRequest_AndLogLevelIsInformation_LogEntryIsAdded()
        {
            var loggingSettings = Services.GetRequiredService<LoggingSettings>();
            loggingSettings.LogLevel = LogLevel.Information;
            var repository = Services.GetRequiredService<IRepository<LogEntry>>();
            var url = $"/api/{nameof(Moon)}";
            var countBefore = (await repository.GetAsync()).Where(log => log.LogLevel == LogLevel.Information.ToString()).Count();

            await Client.GetAsync(url);

            var countAfter = (await repository.GetAsync()).Where(log => log.LogLevel == LogLevel.Information.ToString()).Count();
            Assert.True(countBefore < countAfter);
        }

        [Fact]
        public async Task WhenDoingGetRequest_AndLogLevelIsHigherThanInformation_LogEntryIsNotAdded()
        {
            var loggingSettings = Services.GetRequiredService<LoggingSettings>();
            loggingSettings.LogLevel = LogLevel.Warning;
            var repository = Services.GetRequiredService<IRepository<LogEntry>>();
            var url = $"/api/{nameof(Moon)}";
            var countBefore = (await repository.GetAsync()).Where(log => log.LogLevel == LogLevel.Information.ToString()).Count();

            await Client.GetAsync(url);

            var countAfter = (await repository.GetAsync()).Where(log => log.LogLevel == LogLevel.Information.ToString()).Count();
            Assert.Equal(countBefore, countAfter);
        }

        [Fact]
        public async Task WhenDoingGetRequest_AndLogLevelIsHigherThanTrace_LogEntryIsNotAdded()
        {
            var loggingSettings = Services.GetRequiredService<LoggingSettings>();
            loggingSettings.LogLevel = LogLevel.Debug;
            var repository = Services.GetRequiredService<IRepository<LogEntry>>();
            var url = $"/api/{nameof(Moon)}";
            var countBefore = (await repository.GetAsync()).Where(log => log.LogLevel == LogLevel.Trace.ToString()).Count();

            await Client.GetAsync(url);

            var countAfter = (await repository.GetAsync()).Where(log => log.LogLevel == LogLevel.Trace.ToString()).Count();
            Assert.Equal(countBefore, countAfter);
        }
    }
}
