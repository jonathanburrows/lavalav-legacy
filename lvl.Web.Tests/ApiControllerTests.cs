using lvl.Ontology;
using lvl.Repositories;
using lvl.TestDomain;
using lvl.Web.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public async Task WhenRequestingEntities_AllReturnedEntitiesAreOfGivenType()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon { });
            await repository.CreateAsync(new Moon { });
            await repository.CreateAsync(new Moon { });
            var getUrl = $"/api/{nameof(Moon)}";

            var getResult = await Client.GetAsync(getUrl);
            var serialized = await getResult.Content.ReadAsStringAsync();
            var entities = JsonConvert.DeserializeObject<List<Moon>>(serialized);

            Assert.IsAssignableFrom<IEnumerable<Moon>>(entities);
        }

        [Fact]
        public async Task WhenRequestingEntities_IfTypeIsntMapped_ThrowsInvalidOperationException()
        {
            var getUrl = $"/api/madeUpEntity";

            await Assert.ThrowsAsync<InvalidOperationException>(() => Client.GetAsync(getUrl));
        }

        [Fact]
        public async Task WhenRequestingEntity_WithMatchingId_Returns200()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var entity = await repository.CreateAsync(new Moon { });
            var getUrl = $"/api/{nameof(Moon)}/{entity.Id}";

            var getResponse = await Client.GetAsync(getUrl);

            Assert.Equal(getResponse.StatusCode, HttpStatusCode.OK);
        }

        [Fact]
        public async Task WhenRequestingEntity_WithNoMatchingId_Returns500()
        {
            var getUrl = $"{Client.BaseAddress}api/{nameof(Moon)}/{int.MaxValue}";

            await Assert.ThrowsAsync<InvalidOperationException>(() => Client.GetAsync(getUrl));
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

        [Fact]
        public async Task WhenPosting_EntityIsStoredPersistently()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var postUrl = $"/api/{nameof(Moon)}";
            var posting = new Moon { };
            var postingSerialized = JsonConvert.SerializeObject(posting);
            var postingContent = new StringContent(postingSerialized);

            var postResult = await Client.PostAsync(postUrl, postingContent);
            var resultSerialized = await postResult.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Moon>(resultSerialized);
            var posted = await repository.GetAsync(result.Id);

            Assert.NotNull(posted);
        }

        [Fact]
        public async Task WhenPosting_AndEntityIsNull_ArgumentNullExceptionIsThrown()
        {
            var postUrl = $"/api/{nameof(Moon)}";
            var nullContent = new StringContent("");

            await Assert.ThrowsAsync<ArgumentNullException>(() => Client.PostAsync(postUrl, nullContent));
        }

        [Fact]
        public async Task WhenPosting_AndEntityTypeIsntMapped_InvalidOperationExceptionIsThrown()
        {
            var postUrl = $"/api/{nameof(UnmappedEntity)}";
            var posting = new Moon { };
            var postingSerialized = JsonConvert.SerializeObject(posting);
            var postingContent = new StringContent(postingSerialized);

            await Assert.ThrowsAsync<InvalidOperationException>(() => Client.PostAsync(postUrl, postingContent));
        }

        [Fact]
        public async Task WhenPosting_AndEntityCantBeDeserializedToType_JsonSerializationExceptionIsThrown()
        {
            var postUrl = $"/api/{nameof(Moon)}";
            var postingContent = new StringContent(@"{invalid: ""true""}");

            await Assert.ThrowsAsync<JsonSerializationException>(() => Client.PostAsync(postUrl, postingContent));
        }

        [Fact]
        public async Task WhenPosting_AndEntityAlreadyHasId_InvalidOperationExceptionIsThrown()
        {
            var postUrl = $"/api/{nameof(Moon)}";
            var posting = new Moon { Id = 1 };
            var postingSerialized = JsonConvert.SerializeObject(posting);
            var postingContent = new StringContent(postingSerialized);

            await Assert.ThrowsAsync<InvalidOperationException>(() => Client.PostAsync(postUrl, postingContent));
        }

        [Fact]
        public async Task WhenPutting_EntityPropertiesAreStoredPersistently()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var putting = await repository.CreateAsync(new Moon { Name = "Old Moon" });

            putting.Name = "New Moon";
            var putUrl = $"/api/{nameof(Moon)}";
            var puttingSerialized = JsonConvert.SerializeObject(putting);
            var puttingContent = new StringContent(puttingSerialized);

            var putResult = await Client.PutAsync(putUrl, puttingContent);
            var resultSerialized = await putResult.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Moon>(resultSerialized);
            var putted = await repository.GetAsync(result.Id);

            Assert.Equal(putting.Name, putted.Name);
        }

        [Fact]
        public async Task WhenPutting_AndEntityIsNull_ArgumentNullExceptionIsThrown()
        {
            var putUrl = $"/api/{nameof(Moon)}";
            var nullContent = new StringContent("");

            await Assert.ThrowsAsync<ArgumentNullException>(() => Client.PutAsync(putUrl, nullContent));
        }

        [Fact]
        public async Task WhenPutting_AndEntityTypeIsntMapped_InvalidOperationExceptionIsThrown()
        {
            var putUrl = $"/api/{nameof(UnmappedEntity)}";
            var putting = new Moon { };
            var puttingSerialized = JsonConvert.SerializeObject(putting);
            var puttingContent = new StringContent(puttingSerialized);

            await Assert.ThrowsAsync<InvalidOperationException>(() => Client.PutAsync(putUrl, puttingContent));
        }

        [Fact]
        public async Task WhenPutting_AndEntityCantBeDeserializedToType_JsonSerializationExceptionIsThrown()
        {
            var putUrl = $"/api/{nameof(Moon)}";
            var puttingContent = new StringContent(@"{invalid: ""true""}");

            await Assert.ThrowsAsync<JsonSerializationException>(() => Client.PutAsync(putUrl, puttingContent));
        }

        [Fact]
        public async Task WhenPutting_AndEntityDoesNotExist_InvalidOperationExceptionIsThrown()
        {
            var putUrl = $"/api/{nameof(Moon)}";
            var putting = new Moon { Id = int.MaxValue };
            var puttingSerialized = JsonConvert.SerializeObject(putting);
            var puttingContent = new StringContent(puttingSerialized);

            await Assert.ThrowsAsync<InvalidOperationException>(() => Client.PutAsync(putUrl, puttingContent));
        }

        private class UnmappedEntity : IEntity
        {
            public int Id { get; set; }
        }
    }
}
