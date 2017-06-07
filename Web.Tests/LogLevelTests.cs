using lvl.Repositories;
using lvl.TestDomain;
using lvl.Web.Logging;
using lvl.Web.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using lvl.TestWebSite.Fixtures;
using Xunit;

namespace lvl.Web.Tests
{
    [Collection(nameof(WebCollection))]
    public class LogLevelTests
    {
        private HttpClient Client { get; }
        private IServiceProvider Services { get; }

        public LogLevelTests(WebHostFixture<TestWebSite.Startup> webHostFixture)
        {
            if (webHostFixture == null)
            {
                throw new ArgumentNullException(nameof(webHostFixture));
            }

            Client = webHostFixture.Client;
            Services = webHostFixture.Services;
        }

        [Fact]
        public async Task It_will_add_log_entry_when_performing_request_and_log_level_is_debug()
        {
            var loggingSettings = Services.GetRequiredService<LoggingOptions>();
            loggingSettings.LogLevel = LogLevel.Debug;
            var repository = Services.GetRequiredService<IRepository<LogEntry>>();
            var url = $"/api/{nameof(Moon)}";
            var countBefore = (await repository.GetAsync()).Count(log => log.LogLevel == LogLevel.Debug.ToString());

            await Client.GetAsync(url);

            var countAfter = (await repository.GetAsync()).Count(log => log.LogLevel == LogLevel.Debug.ToString());
            Assert.True(countBefore < countAfter);
        }

        [Fact]
        public async Task It_will_not_add_log_entry_when_performing_request_and_log_level_is_higher_than_debug()
        {
            var loggingSettings = Services.GetRequiredService<LoggingOptions>();
            loggingSettings.LogLevel = LogLevel.Information;
            var repository = Services.GetRequiredService<IRepository<LogEntry>>();
            var url = $"/api/{nameof(Moon)}";
            var countBefore = (await repository.GetAsync()).Count(log => log.LogLevel == LogLevel.Debug.ToString());

            await Client.GetAsync(url);

            var countAfter = (await repository.GetAsync()).Count(log => log.LogLevel == LogLevel.Debug.ToString());
            Assert.Equal(countBefore, countAfter);
        }

        [Fact]
        public async Task It_will_add_log_entry_when_performing_get_and_log_level_is_information()
        {
            var loggingSettings = Services.GetRequiredService<LoggingOptions>();
            loggingSettings.LogLevel = LogLevel.Information;
            var repository = Services.GetRequiredService<IRepository<LogEntry>>();
            var url = $"/api/{nameof(Moon)}";
            var countBefore = (await repository.GetAsync()).Count(log => log.LogLevel == LogLevel.Information.ToString());

            await Client.GetAsync(url);

            var countAfter = (await repository.GetAsync()).Count(log => log.LogLevel == LogLevel.Information.ToString());
            Assert.True(countBefore < countAfter);
        }

        [Fact]
        public async Task It_will_not_add_log_entry_when_performing_get_and_log_level_is_warning()
        {
            var loggingSettings = Services.GetRequiredService<LoggingOptions>();
            loggingSettings.LogLevel = LogLevel.Warning;
            var repository = Services.GetRequiredService<IRepository<LogEntry>>();
            var url = $"/api/{nameof(Moon)}";
            var countBefore = (await repository.GetAsync()).Count(log => log.LogLevel == LogLevel.Information.ToString());

            await Client.GetAsync(url);

            var countAfter = (await repository.GetAsync()).Count(log => log.LogLevel == LogLevel.Information.ToString());
            Assert.Equal(countBefore, countAfter);
        }

        [Fact]
        public async Task It_will_not_add_log_level_when_perorming_get_and_log_level_is_debug()
        {
            var loggingSettings = Services.GetRequiredService<LoggingOptions>();
            loggingSettings.LogLevel = LogLevel.Debug;
            var repository = Services.GetRequiredService<IRepository<LogEntry>>();
            var url = $"/api/{nameof(Moon)}";
            var countBefore = (await repository.GetAsync()).Count(log => log.LogLevel == LogLevel.Trace.ToString());

            await Client.GetAsync(url);

            var countAfter = (await repository.GetAsync()).Count(log => log.LogLevel == LogLevel.Trace.ToString());
            Assert.Equal(countBefore, countAfter);
        }
    }
}
