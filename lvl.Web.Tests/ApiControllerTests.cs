using lvl.Repositories;
using lvl.TestDomain;
using lvl.Web.Tests.Fixtures;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
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

        //[Fact]
        //public async Task WhenRequestingEntity_WithNoMatchingId_Returns500()
        //{
        //    var url = $"/api/{nameof(Moon)}/{int.MaxValue}";

        //    var getResponse = await Client.GetAsync(url);

        //    Assert.Equal(getResponse.StatusCode, HttpStatusCode.InternalServerError);
        //}

        //[Theory]
        //[InlineData("POST")]
        //[InlineData("PUT")]
        //[InlineData("DELETE")]
        //public void WhenRequestingEntity_WithoutGet_Returns404(string httpMethod)
        //{
        //    Assert.True(false);
        //}

        //public void WhenRequestingEntity_AndMatchingEntity_SerializedObjectIsReturned()
        //{
        //    Assert.True(false);
        //}

        //[Fact]
        //public void WhenRequestingEntity_AndTypeIsntMapped_Returns500()
        //{

        //}
    }
}
