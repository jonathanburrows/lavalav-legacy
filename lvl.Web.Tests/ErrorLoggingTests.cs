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
    [Collection(WebCollection.Name)]
    public class ErrorLoggingTests
    {
        private HttpClient Client { get; }
        private IServiceProvider Services { get; }

        public ErrorLoggingTests(WebHostFixture<TestWebSite.Startup> webHostFixture)
        {
            Client = webHostFixture.Client;
            Services = webHostFixture.Services;
        }

        [Fact]
        public async Task WhenPerformingGetRequest_AndErrorOccurs_LogEntryIsAdded()
        {
            var repository = Services.GetRequiredService<IRepository<LogEntry>>();
            var countBefore = (await repository.GetAsync()).Count(log => log.LogLevel == LogLevel.Error.ToString());
            var url = $"/api/{nameof(Moon)}/{int.MaxValue}";

            try
            {
                await Client.GetAsync(url);
            }
            catch
            {
                // ignored
            }

            var countAfter = (await repository.GetAsync()).Count(log => log.LogLevel == LogLevel.Error.ToString());
            Assert.True(countBefore < countAfter);
        }

        [Fact]
        public async Task WhenPerformingGetRequest_AndFinishesSuccessfully_NoErrorLogEntryIsAdded()
        {
            var repository = Services.GetRequiredService<IRepository<LogEntry>>();
            var countBefore = (await repository.GetAsync()).Count(log => log.LogLevel == LogLevel.Error.ToString());
            var url = $"/api/{nameof(Moon)}";

            await Client.GetAsync(url);

            var countAfter = (await repository.GetAsync()).Count(log => log.LogLevel == LogLevel.Error.ToString());
            Assert.Equal(countAfter, countBefore);
        }
    }
}
