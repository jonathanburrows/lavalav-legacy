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
using lvl.TestWebSite.Fixtures;
using Xunit;
using FluentNHibernate.Data;

namespace lvl.Web.Tests
{
    [Collection(nameof(WebCollection))]
    public class ApiControllerTests
    {
        private HttpClient Client { get; }
        private IServiceProvider Services { get; }

        public ApiControllerTests(WebHostFixture<TestWebSite.Startup> webHostFixture)
        {
            if (webHostFixture == null)
            {
                throw new ArgumentNullException(nameof(webHostFixture));
            }

            Client = webHostFixture.Client;
            Services = webHostFixture.Services;
        }

        [Fact]
        public async Task Get_will_only_entities_of_a_given_type()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            await repository.CreateAsync(new Moon());
            await repository.CreateAsync(new Moon());
            var getUrl = $"/api/{nameof(Moon)}";

            var getResult = await Client.GetAsync(getUrl);
            var serialized = await getResult.Content.ReadAsStringAsync();
            var entities = JsonConvert.DeserializeObject<List<Moon>>(serialized);

            Assert.IsAssignableFrom<IEnumerable<Moon>>(entities);
        }

        [Fact]
        public async Task Get_will_throw_invalid_operation_exception_when_type_isnt_mapped()
        {
            const string getUrl = "/api/madeUpEntity";

            var getResult = await Client.GetAsync(getUrl);

            Assert.False(getResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Get_will_be_successful_when_theres_an_entity_with_matching_id()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var entity = await repository.CreateAsync(new Moon());
            var getUrl = $"/api/{nameof(Moon)}/{entity.Id}";

            var getResponse = await Client.GetAsync(getUrl);

            Assert.Equal(getResponse.StatusCode, HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_will_fail_when_theres_no_entity_with_matching_id()
        {
            var getUrl = $"{Client.BaseAddress}api/{nameof(Moon)}/{int.MaxValue}";

            var getResult = await Client.GetAsync(getUrl);

            Assert.False(getResult.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData("POST")]
        [InlineData("PUT")]
        [InlineData("DELETE")]
        public async Task It_will_return_404_when_attempting_get_with_non_get_method(string httpMethod)
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var fetching = await repository.CreateAsync(new Moon());
            var getUrl = $"/api/{nameof(Moon)}/{fetching.Id}";
            var getMessage = new HttpRequestMessage(new HttpMethod(httpMethod), getUrl);

            var getResponse = await Client.SendAsync(getMessage);

            Assert.Equal(getResponse.StatusCode, HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_will_return_serialized_object_when_there_is_matching_entity()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var fetching = await repository.CreateAsync(new Moon());
            var getUrl = $"/api/{nameof(Moon)}/{fetching.Id}";

            var fetched = await Client.GetAsync(getUrl);
            var serialized = await fetched.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<Moon>(serialized);

            Assert.Equal(fetching.Id, deserialized.Id);
        }

        [Fact]
        public async Task Get_single_will_return_500_when_type_isnt_mapped()
        {
            var getUrl = "/api/madeUpEntity/1";

            var getResult = await Client.GetAsync(getUrl);

            Assert.False(getResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Posting_will_persist_the_entity()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var postUrl = $"/api/{nameof(Moon)}";
            var posting = new Moon();
            var postingSerialized = JsonConvert.SerializeObject(posting);
            var postingContent = new StringContent(postingSerialized);

            var postResult = await Client.PostAsync(postUrl, postingContent);
            var resultSerialized = await postResult.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Moon>(resultSerialized);
            var posted = await repository.GetAsync(result.Id);

            Assert.NotNull(posted);
        }

        [Fact]
        public async Task Post_will_throw_argument_null_exception_when_empty_payload_is_sent()
        {
            var postUrl = $"/api/{nameof(Moon)}";
            var nullContent = new StringContent("");

            var postResult = await Client.PostAsync(postUrl, nullContent);

            Assert.False(postResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Post_will_throw_invalid_operation_exception_when_type_isnt_mapped()
        {
            var postUrl = $"/api/{nameof(UnmappedEntity)}";
            var posting = new Moon();
            var postingSerialized = JsonConvert.SerializeObject(posting);
            var postingContent = new StringContent(postingSerialized);

            var postResult = await Client.PostAsync(postUrl, postingContent);

            Assert.False(postResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Post_will_throw_json_serialization_exception_when_entity_additional_inforamtion_is_provided()
        {
            var postUrl = $"/api/{nameof(Moon)}";
            var postingContent = new StringContent(@"{invalid: ""true""}");

            var postResult = await Client.PostAsync(postUrl, postingContent);

            Assert.False(postResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Post_will_throw_invalid_operation_exception_when_entity_already_has_id()
        {
            var postUrl = $"/api/{nameof(Moon)}";
            var posting = new Moon { Id = 1 };
            var postingSerialized = JsonConvert.SerializeObject(posting);
            var postingContent = new StringContent(postingSerialized);

            var postResult = await Client.PostAsync(postUrl, postingContent);

            Assert.False(postResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Put_will_persist_changed_properties()
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
        public async Task Put_will_throw_argument_null_exception_when_payload_is_empty()
        {
            var putUrl = $"/api/{nameof(Moon)}";
            var nullContent = new StringContent("");

            var putResult = await Client.PutAsync(putUrl, nullContent);

            Assert.False(putResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Put_will_throw_invalid_operation_exception_when_entity_type_isnt_mapped()
        {
            var putUrl = $"/api/{nameof(UnmappedEntity)}";
            var putting = new Moon();
            var puttingSerialized = JsonConvert.SerializeObject(putting);
            var puttingContent = new StringContent(puttingSerialized);

            var putResult = await Client.PutAsync(putUrl, puttingContent);

            Assert.False(putResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Put_will_throw_json_serialization_exception_when_extra_properties_are_sent()
        {
            var putUrl = $"/api/{nameof(Moon)}";
            var puttingContent = new StringContent(@"{invalid: ""true""}");

            var putResult = await Client.PutAsync(putUrl, puttingContent);

            Assert.False(putResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Put_will_throw_invalid_operation_exception_when_no_matching_entity_exists()
        {
            var putUrl = $"/api/{nameof(Moon)}";
            var putting = new Moon { Id = int.MaxValue };
            var puttingSerialized = JsonConvert.SerializeObject(putting);
            var puttingContent = new StringContent(puttingSerialized);

            var putResult = await Client.PutAsync(putUrl, puttingContent);

            Assert.False(putResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Delete_will_persist_the_removal_of_the_entity()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var deleting = await repository.CreateAsync(new Moon());
            var deletingSerialized = JsonConvert.SerializeObject(deleting);
            var deletingContent = new StringContent(deletingSerialized);
            var deletingMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Delete,
                Content = deletingContent
            };

            var deleteResult = await Client.SendAsync(deletingMessage);
            var deleteSerialized = await deleteResult.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Moon>(deleteSerialized);
            var deleted = await repository.GetAsync(result.Id);

            Assert.Null(deleted);
        }

        [Fact]
        public async Task Delete_will_throw_argument_null_exception_when_payload_is_null()
        {
            var deletingMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Delete,
                Content = new StringContent(string.Empty)
            };

            var deleteResponse = await Client.SendAsync(deletingMessage);
            Assert.False(deleteResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Delete_will_throw_invalid_operation_exception_when_type_isnt_mapped()
        {
            var deleting = new Moon();
            var deletingSerialized = JsonConvert.SerializeObject(deleting);
            var deletingContent = new StringContent(deletingSerialized);
            var deletingMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/NonExistant"),
                Method = HttpMethod.Delete,
                Content = deletingContent
            };

            var deleteResult = await Client.SendAsync(deletingMessage);
            Assert.False(deleteResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Delete_will_throw_json_serialization_exception_when_extra_properties_are_provided()
        {
            var deletingMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Delete,
                Content = new StringContent(@"{invalid: ""true""}")
            };

            var deleteResponse = await Client.SendAsync(deletingMessage);
            Assert.False(deleteResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Delete_will_throw_invalid_operation_exception_when_no_matching_entity_is_found()
        {
            var deleting = new Moon { Id = int.MaxValue };
            var deletingSerialized = JsonConvert.SerializeObject(deleting);
            var deletingContent = new StringContent(deletingSerialized);
            var deletingMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Delete,
                Content = deletingContent
            };

            var deleteResult = await Client.SendAsync(deletingMessage);
            Assert.False(deleteResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task It_will_throw_an_error_when_getting_single_entity_with_hide_from_api_attribute()
        {
            var repository = Services.GetRequiredService<IRepository<NasaApplication>>();
            var hiddenEntity = await repository.CreateAsync(new NasaApplication());
            var url = $"{Client.BaseAddress}api/{nameof(NasaApplication)}/{hiddenEntity.Id}";

            var getResult = await Client.GetAsync(url);

            Assert.False(getResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task It_will_throw_an_error_when_getting_multiple_entities_with_hide_from_api_attribute()
        {
            var url = $"{Client.BaseAddress}api/{nameof(NasaApplication)}";

            var getResult = await Client.GetAsync(url);

            Assert.False(getResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task It_will_throw_an_error_when_creating_entity_with_hide_from_api_attribute()
        {
            var postUrl = $"/api/{nameof(NasaApplication)}";
            var posting = new NasaApplication();
            var postingSerialized = JsonConvert.SerializeObject(posting);
            var postingContent = new StringContent(postingSerialized);

            var postResult = await Client.PostAsync(postUrl, postingContent);

            Assert.False(postResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task It_will_throw_an_error_when_updating_entity_with_hide_from_api_attribute()
        {
            var repository = Services.GetRequiredService<IRepository<NasaApplication>>();
            var putting = await repository.CreateAsync(new NasaApplication());
            var putUrl = $"/api/{nameof(NasaApplication)}";
            var puttingSerialized = JsonConvert.SerializeObject(putting);
            var postingContent = new StringContent(puttingSerialized);

            var postResult = await Client.PutAsync(putUrl, postingContent);

            Assert.False(postResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task It_will_throw_an_error_when_deleting_entity_with_hide_from_api_attribute()
        {
            var repository = Services.GetRequiredService<IRepository<NasaApplication>>();
            var deleting = await repository.CreateAsync(new NasaApplication());
            var deleteUri = new Uri($"{Client.BaseAddress}api/{nameof(NasaApplication)}");
            var deletingSerialized = JsonConvert.SerializeObject(deleting);
            var deletingContent = new StringContent(deletingSerialized);
            var deletingMessage = new HttpRequestMessage
            {
                RequestUri = deleteUri,
                Method = HttpMethod.Delete,
                Content = deletingContent
            };

            var deleteResult = await Client.SendAsync(deletingMessage);

            Assert.False(deleteResult.IsSuccessStatusCode);
        }

        // ReSharper disable once ClassNeverInstantiated.Local Used by reflection
        private class UnmappedEntity : Entity { }
    }
}
