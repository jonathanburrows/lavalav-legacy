using lvl.Oidc.AuthorizationServer.Models;
using lvl.Oidc.AuthorizationServer.Stores;
using lvl.Oidc.AuthorizationServer.Tests.Fixtures;
using lvl.Repositories;
using lvl.TestWebSite.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace lvl.Oidc.AuthorizationServer.Tests
{
    [Collection(nameof(OidcAuthorizationServerCollection))]
    public class UserControllerTests
    {
        private IRepository<User> UserRepository { get; }
        private UserStore UserStore { get; }
        private HttpClient Client { get; }

        public UserControllerTests(WebHostFixture<Startup> webHostFixture)
        {
            var serviceProvider = webHostFixture.Services;
            UserRepository = serviceProvider.GetRequiredService<IRepository<User>>();
            UserStore = serviceProvider.GetRequiredService<UserStore>();
            Client = webHostFixture.Client;
        }

        [Fact]
        public async Task It_will_return_model_error_when_no_username_provided()
        {
            var postUrl = $"/oidc/user";
            var postingContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["password"] = "password"
            });

            var postResult = await Client.PostAsync(postUrl, postingContent);
            var resultSerialized = await postResult.Content.ReadAsStringAsync();
            var modelErrors = JsonConvert.DeserializeObject<CreateUserModelErrorResponse>(resultSerialized);

            Assert.NotEmpty(modelErrors.Username);
        }

        [Fact]
        public async Task It_will_return_model_error_when_username_taken()
        {
            await UserStore.AddLocalUserAsync("create-user-username-taken", "password");

            var postUrl = $"/oidc/user";
            var postingContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["username"] = "create-user-username-taken",
                ["password"] = "password"
            });

            var postResult = await Client.PostAsync(postUrl, postingContent);
            var resultSerialized = await postResult.Content.ReadAsStringAsync();
            var modelErrors = JsonConvert.DeserializeObject<CreateUserModelErrorResponse>(resultSerialized);

            Assert.NotEmpty(modelErrors.Username);
        }

        [Fact]
        public async Task It_will_return_model_error_when_password_not_given()
        {
            var postUrl = $"/oidc/user";
            var postingContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["username"] = "username"
            });

            var postResult = await Client.PostAsync(postUrl, postingContent);
            var resultSerialized = await postResult.Content.ReadAsStringAsync();
            var modelErrors = JsonConvert.DeserializeObject<CreateUserModelErrorResponse>(resultSerialized);

            Assert.NotEmpty(modelErrors.Password);
        }

        [Fact]
        public async Task It_will_return_added_user()
        {
            var postUrl = $"/oidc/user";
            var postingContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["username"] = "create-user-valid",
                ["password"] = "password"
            });

            var postResult = await Client.PostAsync(postUrl, postingContent);
            var resultSerialized = await postResult.Content.ReadAsStringAsync();
            var addedUser = JsonConvert.DeserializeObject<User>(resultSerialized);

            Assert.NotNull(addedUser);
        }

        [Fact]
        public async Task It_will_persist_added_user()
        {
            var postUrl = $"/oidc/user";
            var postingContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["username"] = "create-user-persisted",
                ["password"] = "password"
            });

            var postResult = await Client.PostAsync(postUrl, postingContent);
            var resultSerialized = await postResult.Content.ReadAsStringAsync();
            var addedUser = JsonConvert.DeserializeObject<User>(resultSerialized);

            var matchingUser = UserRepository.GetAsync(addedUser.Id);
            Assert.NotNull(matchingUser);
        }

        private class CreateUserModelErrorResponse
        {
            [JsonProperty("username")]
            public List<string> Username { get; set; }

            [JsonProperty("password")]
            public List<string> Password { get; set; }
        }
    }
}
