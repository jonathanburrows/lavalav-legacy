using lvl.Repositories;
using lvl.TestDomain;
using lvl.Web.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace lvl.Web.Tests
{
    [Collection(nameof(ApiControllerTests))]
    public class ApiControllerTests : IClassFixture<WebHostFixture>
    {
        private HttpClient Client { get; }
        private IServiceProvider Services { get; }

        public ApiControllerTests(WebHostFixture webHostFixture)
        {
            Client = webHostFixture.Client;
            Services = webHostFixture.Services;
        }

        [Fact]
        public async Task WhenRequestingEntity_WithMatchingId_Returns200()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var entity = await repository.CreateAsync(new Moon { });
            var url = $"/api/{nameof(Moon)}/{entity.Id}";

            var getResponse = await Client.GetAsync(url);

            Assert.Equal(getResponse.StatusCode, HttpStatusCode.OK);
        }

        [Fact]
        public async Task WhenRequestingEntity_WithNoMatchingId_Returns500()
        {
            var getUrl = $"{Client.BaseAddress}api/{nameof(Moon)}/{int.MaxValue}";

            await Assert.ThrowsAsync<InvalidOperationException>(async() => await Client.GetAsync(getUrl));
        }

        [Theory]
        [InlineData("POST")]
        [InlineData("PUT")]
        [InlineData("DELETE")]
        public async Task WhenRequestingEntity_WithoutGet_Returns404(string httpMethod)
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var fetching = await repository.CreateAsync(new Moon { });
            var getUrl = $"/api/{nameof(Moon)}/{fetching.Id}";
            var getMessage = new HttpRequestMessage(new HttpMethod(httpMethod), getUrl);

            var getResponse = await Client.SendAsync(getMessage);

            Assert.Equal(getResponse.StatusCode, HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenRequestingEntity_AndMatchingEntity_SerializedObjectIsReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var fetching = await repository.CreateAsync(new Moon { });
            var getUrl = $"/api/{nameof(Moon)}/{fetching.Id}";

            var fetched = await Client.GetAsync(getUrl);
            var serialized = await fetched.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<Moon>(serialized);

            Assert.Equal(fetching.Id, deserialized.Id);
        }

        [Fact]
        public async Task WhenRequestingEntity_AndTypeIsntMapped_Returns500()
        {
            var getUrl = "/api/madeUpEntity/1";

            await Assert.ThrowsAsync<InvalidOperationException>(() => Client.GetAsync(getUrl));
        }
    }
}
