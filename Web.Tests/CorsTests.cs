﻿using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Cors.Infrastructure;
using lvl.TestDomain;
using System.Threading.Tasks;
using lvl.TestWebSite;
using System.Net.Http;
using System;
using System.Net;
using System.Linq;
using lvl.TestWebSite.Fixtures;
using lvl.Web.Tests.Fixtures;

namespace lvl.Web.Tests
{
    [Collection(nameof(WebCollection))]
    public class CorsTests
    {
        private static Microsoft.Extensions.DependencyInjection.CorsOptions Settings { get; } = new Microsoft.Extensions.DependencyInjection.CorsOptions
        {
            AllowMethods = new[] { "GET", "POST", "PUT", "DELETE", "NON-STANDARD" },
            AllowOrigins = new[] { "http://localhost/" },
            AllowHeaders = new[] { "ALLOWED-HEADER" },
            ExposedHeaders = new[] { "ALLOWED-EXPOSED-HEADER" }
        };

        private HttpClient Client { get; }

        public CorsTests(WebHostFixture<CorsStartup> webHostFixture)
        {
            Client = webHostFixture?.Client ?? throw new ArgumentNullException(nameof(webHostFixture));
        }

        [Theory]
        [InlineData("GET")]
        [InlineData("POST")]
        [InlineData("PUT")]
        [InlineData("DELETE")]
        public void It_will_return_cors_response_for_all_http_methods(string method)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.Origin, Client.BaseAddress.ToString());
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, method);

            // Since Theories dont adhere to the TestCollection rules, the call was made synchronous.
            var response = Client.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Theory]
        [InlineData("PUT")]
        [InlineData("DELETE")]
        [InlineData("NON-STANDARD")]
        public void It_will_respond_when_non_standard_methdods_are_used(string nonStandardMethod)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.Origin, Client.BaseAddress.ToString());
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, nonStandardMethod);

            // due to theories not adhering to test collection behaviour, calls are made synchronously.
            var response = Client.SendAsync(request).Result;
            var allowedMethods = response.Headers.GetValues(CorsConstants.AccessControlAllowMethods);

            Assert.Contains(nonStandardMethod, allowedMethods);
        }

        [Fact]
        public async Task It_will_not_return_allowed_methods_when_unsupported_methods_are_used()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.Origin, Client.BaseAddress.ToString());
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, "UNSUPPORTED");

            var response = await Client.SendAsync(request);

            var containsMethods = response.Headers.Any(header => header.Key == CorsConstants.AccessControlAllowMethods);
            Assert.False(containsMethods);
        }

        [Fact]
        public async Task Requesting_options_will_return_404_when_no_method_is_provided()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.Origin, Client.BaseAddress.ToString());

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Requesting_options_will_no_give_origins_when_origin_is_not_allowed()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.Origin, "http://example.com/");
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, HttpMethod.Get.Method);

            var response = await Client.SendAsync(request);

            var containsOrigin = response.Headers.Any(header => header.Key == CorsConstants.AccessControlAllowOrigin);
            Assert.False(containsOrigin);
        }

        [Fact]
        public async Task Requesting_options_will_populate_origins_when_requests_origin_is_allowed()
        {
            var origin = Client.BaseAddress.ToString();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.Origin, origin);
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, HttpMethod.Get.Method);

            var response = await Client.SendAsync(request);

            var containsOrigin = response.Headers.Any(h => h.Key == CorsConstants.AccessControlAllowOrigin && h.Value.Contains(origin));
            Assert.True(containsOrigin);
        }

        [Fact]
        public async Task Requesting_options_will_return_404_when_no_origin_is_provide()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, HttpMethod.Get.Method);

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Requesting_options_will_not_privde_headers_when_origin_isnt_allowed_but_method_is()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.Origin, "http://example.com/");
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, "NON-STANDARD");

            var response = await Client.SendAsync(request);

            var containsMethods = response.Headers.Any(header => header.Key == CorsConstants.AccessControlAllowMethods);
            Assert.False(containsMethods);
        }

        [Fact]
        public async Task Requesting_options_will_not_return_headers_when_headers_arent_allowed()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.Origin, Client.BaseAddress.ToString());
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, HttpMethod.Get.Method);
            request.Headers.Add(CorsConstants.AccessControlRequestHeaders, "NON-ALLOWED-HEADER");

            var response = await Client.SendAsync(request);

            Assert.Empty(response.Headers);
        }

        [Fact]
        public async Task Requesting_options_will_return_headers_when_request_headers_are_allowed()
        {
            var header = "ALLOWED-HEADER";
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.Origin, Client.BaseAddress.ToString());
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, HttpMethod.Get.Method);
            request.Headers.Add(CorsConstants.AccessControlRequestHeaders, header);

            var response = await Client.SendAsync(request);

            var containsHeader = response.Headers.Any(h => h.Key == CorsConstants.AccessControlAllowHeaders && h.Value.Contains(header));
            Assert.True(containsHeader);
        }

        [Fact]
        public async Task Standard_request_will_return_exposed_headers_when_using_allowed_exposed_headers()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Get
            };
            request.Headers.Add(CorsConstants.Origin, Client.BaseAddress.ToString());
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, HttpMethod.Get.Method);

            var response = await Client.SendAsync(request);

            var containsExposedHeaders = response.Headers.Any(h => h.Key == CorsConstants.AccessControlExposeHeaders && h.Value.Contains("ALLOWED-EXPOSED-HEADER"));
            Assert.True(containsExposedHeaders);
        }

        public class CorsStartup : Startup
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                base.ConfigureServices(services);
                services.AddSingleton(Settings);
            }
        }
    }
}
