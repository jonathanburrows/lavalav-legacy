using lvl.Repositories;
using lvl.TestDomain;
using lvl.Web.Logging;
using lvl.Web.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using lvl.Repositories.Querying;
using lvl.TestWebSite.Fixtures;
using Xunit;

namespace lvl.Web.Tests
{
    [Collection(nameof(WebCollection))]
    public class ErrorLoggingTests
    {
        private HttpClient Client { get; }
        private IServiceProvider Services { get; }

        public ErrorLoggingTests(WebHostFixture<TestWebSite.Startup> webHostFixture)
        {
            if (webHostFixture == null)
            {
                throw new ArgumentNullException(nameof(webHostFixture));
            }

            Client = webHostFixture.Client;
            Services = webHostFixture.Services;
        }

        [Fact]
        public async Task It_will_add_log_entry_when_get_request_fails()
        {
            var repository = Services.GetRequiredService<IRepository<LogEntry>>();
            var errorQuery = new Query<LogEntry>().Where(logEntry => logEntry.LogLevel == LogLevel.Error.ToString());
            var errorsBefore = await repository.GetAsync(errorQuery);
            var url = $"/api/{nameof(Moon)}/{int.MaxValue}";

            await Client.GetAsync(url);

            var errorsAfter = await repository.GetAsync(errorQuery);
            Assert.True(errorsBefore.Count < errorsAfter.Count);
        }

        [Fact]
        public async Task It_will_not_add_error_log_when_get_request_is_successful()
        {
            var repository = Services.GetRequiredService<IRepository<LogEntry>>();
            var errorQuery = new Query<LogEntry>().Where(logEntry => logEntry.LogLevel == LogLevel.Error.ToString());
            var url = $"/api/{nameof(Moon)}";
            var errorsBefore = await repository.GetAsync(errorQuery);

            await Client.GetAsync(url);

            var errorsAfter = await repository.GetAsync(errorQuery);
            Assert.Equal(errorsBefore.Count, errorsAfter.Count);
        }

        [Fact]
        public async Task Request_will_return_log_mesage_when_it_fails()
        {
            var getUrl = $"/api/{nameof(Moon)}/{int.MaxValue}";

            var getResult = await Client.GetAsync(getUrl);
            var getContent = await getResult.Content.ReadAsStringAsync();

            Assert.Contains("Could not find", getContent);
        }
    }
}
